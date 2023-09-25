import os
from typing import Iterator, List

import pandas as pd
import tiktoken
from dotenv import load_dotenv
from langchain.chains import LLMChain
from langchain.chat_models import AzureChatOpenAI
from langchain.docstore.document import Document
from langchain.document_loaders import PyPDFLoader
from langchain.document_loaders.base import BaseLoader

# from langchain.document_loaders.merge import MergedDataLoader
from langchain.embeddings.openai import OpenAIEmbeddings
from langchain.llms import AzureOpenAI, OpenAI
from langchain.prompts import PromptTemplate
from langchain.vectorstores import FAISS

load_dotenv()
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
OPENAI_DEPLOYMENT_NAME = os.getenv("OPENAI_DEPLOYMENT_NAME")


def load_pdf(full_path):
    loader = PyPDFLoader(full_path)
    resume_content = loader.load()
    resume_content = Document(
        page_content="\n".join([page.page_content for page in resume_content]),
        metadata=resume_content[0].metadata,
    )
    return resume_content


relevant_fields = "Name,Relevant Experience,Skills,Education,Achievements,Projects,Current Location,Total Experience in years"
extraction_template = """Extract the fields - {relevant_fields} in form of a list:

Content: {content}"""


def extract_fields(file_path):
    llm = AzureChatOpenAI(
        deployment_name=OPENAI_DEPLOYMENT_NAME,
        openai_api_key=OPENAI_API_KEY,
        openai_api_type="azure",
        temperature=0.0,
    )

    extraction_prompt_template = PromptTemplate(
        input_variables=[
            "relevant_fields",
            "content",
        ],
        template=extraction_template,
    )

    doc = load_pdf(file_path)

    evaluation_chain = LLMChain(
        llm=llm,
        prompt=extraction_prompt_template,
    )

    response = evaluation_chain.run(
        {
            "relevant_fields": relevant_fields,
            "content": doc.page_content,
        }
    )
    return response


if __name__ == "__main__":
    file_path = r"C:\Users\nkothapalli\Downloads\EY_Resume_Aakash Suresh.pdf"
    fields = extract_fields(file_path)
    print(fields)
