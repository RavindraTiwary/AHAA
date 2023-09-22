import json
import os
import pickle
from typing import Iterator, List

import joblib
import pandas as pd
import tiktoken
from dotenv import load_dotenv
from langchain.chains import LLMChain
from langchain.chat_models import AzureChatOpenAI
from langchain.docstore.document import Document
from langchain.document_loaders import PyPDFLoader
from langchain.document_loaders.base import BaseLoader
from langchain.embeddings.openai import OpenAIEmbeddings
from langchain.prompts import PromptTemplate
from langchain.vectorstores import FAISS
from sklearn.feature_extraction.text import TfidfVectorizer

load_dotenv()

OPENAI_ADA_EMBEDDING_DEPLOYMENT_NAME = os.getenv("OPENAI_ADA_EMBEDDING_DEPLOYMENT_NAME")
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
OPENAI_DEPLOYMENT_NAME = os.getenv("OPENAI_DEPLOYMENT_NAME")

skillset_parser = """
Given the {document_type} below, help me extract the technical skills.
Some examples of technical profiles are software engineers/developers, software architects, software testers, data scientists, data engineers, software consultants etc.
In the skillset, include the similar skills, if not present already. For instance, if C# is mentioned, include .NET as well. 
Include only the obvious ones. So, don't include Numpy if only Pandas is mentioned (and not Numpy) since it is not very obvious.

Give the output as a json with the following format:
1. \"is_technical_profile\" (bool),
2. \"skills\"(comma separated string of skills if technical profile otherwise none)

Resume: {document}"""

skillset_parse_template = PromptTemplate(
    input_variables=[
        "document_type",
        "document",
    ],
    template=skillset_parser,
)


def validate_token_size(text: str, token_limit: int = 8192):
    tokenizer = tiktoken.get_encoding("cl100k_base")
    n_tokens = len(tokenizer.encode(text))
    print("Total number of tokens:", n_tokens)
    if n_tokens > token_limit:
        print("Document is exceeding token limit")


def load_pdf(full_path):
    loader = PyPDFLoader(full_path)
    resume_content = loader.load()
    resume_content = Document(
        page_content="\n".join([page.page_content for page in resume_content]),
        metadata=resume_content[0].metadata,
    )
    return resume_content


class SemanticSearch:
    def __init__(self, vector_store=None, embedding_obj=None, vectorizer=None):
        self.embedding_obj = (
            self.create_embedding_object() if embedding_obj is None else embedding_obj
        )
        self.vector_store = vector_store
        self._llm = self.__load_llm()
        self._skillset_parse_chain = LLMChain(
            llm=self._llm,
            prompt=skillset_parse_template,
        )
        if vectorizer is None:
            self._vectorizer = TfidfVectorizer(
                use_idf=True,
                norm="l2",
                binary=False,
                analyzer=lambda set_: list(
                    set([w.strip().lower() for w in set_.split(",")])
                ),
            )
        else:
            self._vectorizer = vectorizer

    def __load_llm(self):
        llm = AzureChatOpenAI(
            deployment_name=OPENAI_DEPLOYMENT_NAME,
            openai_api_key=OPENAI_API_KEY,
            openai_api_type="azure",
            temperature=0.0,
        )
        return llm

    @classmethod
    def from_embedding_path(cls, embedding_path, vectorizer=None):
        embedding_obj = cls().create_embedding_object()
        vector_store = FAISS.load_local(embedding_path, embedding_obj)
        return cls(
            vector_store=vector_store,
            embedding_obj=embedding_obj,
            vectorizer=vectorizer,
        )

    def create_embedding_object(self):
        embedding_obj = OpenAIEmbeddings(
            deployment=OPENAI_ADA_EMBEDDING_DEPLOYMENT_NAME,
            openai_api_type="azure",
            chunk_size=1,
        )
        return embedding_obj

    def create_vector_store(self, docs):
        vector_store = FAISS.from_documents(docs, self.embedding_obj)
        return vector_store

    def get_skillset(self, text, document_type):
        parsed_skills = self._skillset_parse_chain.run(
            {"document": text, "document_type": document_type}
        )
        return parsed_skills

    def save_skillsets(self, skills, metadata):
        print(skills)
        technical_skills = [
            skill["skills"]
            if (skill["skills"] is not None) and (skill["skills"].lower() != "none")
            else ""
            for skill in skills
        ]
        technical_skill_array = self._vectorizer.fit_transform(
            technical_skills
        ).todense()
        df = pd.DataFrame(
            technical_skill_array,
            columns=self._vectorizer.get_feature_names_out(),
            index=metadata,
        )
        df.to_parquet("skills.parquet")

    def generate_skillsets(self, docs, document_type):
        parsed_skills = []
        for doc in docs:
            parsed_skills.append(json.loads(self.get_skillset(doc, document_type)))
        return parsed_skills

    def generate_and_save_embeddings(self, docs, metadata, embedding_path):
        self.vector_store = self.create_vector_store(docs)
        self.vector_store.save_local(embedding_path)
        parsed_skills = self.generate_skillsets(docs, "resume")
        self.save_skillsets(parsed_skills, metadata)
        return self._vectorizer

    def get_most_similar(self, query_vector, doc_vectors, top_k=10):
        print(query_vector.shape, doc_vectors.shape)
        similarity_scores = doc_vectors.dot(query_vector[doc_vectors.columns].values)

        print(similarity_scores)
        return similarity_scores

    def search(self, query, top_k=10):
        parsed_skills = json.loads(self.get_skillset(query, "job_description"))
        if parsed_skills["is_technical_profile"]:
            required_skills = parsed_skills["skills"]
            print(f"{required_skills=}")
            query_vector = self._vectorizer.transform([required_skills]).toarray()
            print(type(query_vector))
            print(query_vector[0])
            print(self._vectorizer.get_feature_names_out())

            doc_vectors = pd.read_parquet("skills.parquet")
            print(doc_vectors)
            print(query_vector)
            query_vector = pd.Series(
                query_vector.tolist()[0],
                index=self._vectorizer.get_feature_names_out(),
            )
            print(query_vector)

            similarity_scores = self.get_most_similar(
                query_vector, doc_vectors, top_k=10
            )
        results = self.vector_store.similarity_search_with_score(query, k=6)
        results = self.vector_store.similarity_search_with_relevance_scores(query, k=10)
        return results, similarity_scores


if __name__ == "__main__":
    folder_path = "../../resumes"
    embedding_path = "../../resume_embeddings"
    folder_path = os.path.abspath(folder_path)
    filepaths = (
        os.path.join(folder_path, filename) for filename in os.listdir(folder_path)
    )
    docs = [load_pdf(filepath) for filepath in filepaths if filepath.endswith(".pdf")]
    metadata = [
        " ".join(os.path.basename(doc.metadata["source"]).split(".")[:-1])
        for doc in docs
    ]

    os.makedirs(embedding_path, exist_ok=True)

    vectorizer = SemanticSearch().generate_and_save_embeddings(
        docs, metadata, embedding_path
    )

    query = """
    Here is the job description, help me find if the resume has all technical skills for this job.
    Higher score should mean better match with the skills.

    Job Title: Data Scientist
    Company: SciData Solutions

    About Us:
    SciData Solutions is a leading technology company known for innovation and excellence in data-driven decision-making.
    We are seeking a talented and experienced Data Scientist to join our dynamic team and help drive our data-driven initiatives.
    If you have strong technical skills, we'd love to hear from you.

    Position Overview:
    We are looking for a Data Scientist with a minimum of 2 years of hands-on experience in data analysis, machine learning, and statistical modeling.
    The ideal candidate will possess a strong skill set in Python, NumPy, Pandas, Scikit Learn, and SQL. You are encouraged to apply even if you don't have experience in all of these areas.

    Key Responsibilities:
    • Collaborate with cross-functional teams to understand business objectives and translate them into data-driven solutions.
    • Collect, preprocess, and analyze datasets to derive actionable insights and build predictive models.
    • Develop and deploy machine learning algorithms and models using Python, Tensorflow/Pytorch, and Scikit Learn.
    • Would be nice if the candidate can write efficient SQL queries to extract and manipulate data from relational databases.

    Qualifications:
    • Bachelor's or Master's degree in a related field (e.g., Computer Science, Statistics, Data Science).
    • Minimum of 2 years of hands-on experience as a Data Scientist.
    • Strong proficiency in Python, NumPy, Pandas, Scikit Learn, Tensorflow/Pytorch, Matplotlib, and SQL."""
    results, similarity_scores = SemanticSearch.from_embedding_path(
        embedding_path,
        vectorizer=vectorizer,
    ).search(query, 10)
    print(results)
    print(similarity_scores)
