import os

from dotenv import load_dotenv
from langchain.chains import LLMChain
from langchain.chat_models import AzureChatOpenAI
from langchain.llms import AzureOpenAI, OpenAI
from langchain.prompts import PromptTemplate

load_dotenv()
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
OPENAI_DEPLOYMENT_NAME = os.getenv("OPENAI_DEPLOYMENT_NAME")

EVALUATION_PROMPT_TEMPLATE = """
You recently interviewed a candidate for a {role} role {experience_template}. 
Given the job description and the interview transcript below, 
{evaluation_template}

Job Description: {job_description}

Interview Transcript: {interview_transcript}

Candidate Performance Summary and Evaluation:"""


def evaluate(
    role,
    experience_template,
    evaluation_template,
    job_description,
    interview_transcript,
):
    llm = AzureChatOpenAI(
        deployment_name=OPENAI_DEPLOYMENT_NAME,
        openai_api_key=OPENAI_API_KEY,
        openai_api_type="azure",
        temperature=0.0,
    )

    evaluation_prompt_template = PromptTemplate(
        input_variables=[
            "role",
            "experience_template",
            "evaluation_template",
            "job_description",
            "interview_transcript",
        ],
        template=EVALUATION_PROMPT_TEMPLATE,
    )

    evaluation_chain = LLMChain(
        llm=llm,
        prompt=evaluation_prompt_template,
    )
    response = evaluation_chain.run(
        {
            "role": role,
            "experience_template": experience_template,
            "evaluation_template": evaluation_template,
            "job_description": job_description,
            "interview_transcript": interview_transcript,
        }
    )
    return response


if __name__ == "__main__":
    role = "data scientist"
    experience_template = "with at least 2 years of experience"

    job_description = """
    Job Title: Data Scientist
    Location: Hyderabad
    Company: SciData Solutions
    About Us:
    SciData Solutions is a leading technology company known for innovation and excellence in data-driven decision-making. We are seeking a talented and experienced Data Scientist to join our dynamic team and help drive our data-driven initiatives. If you are passionate about solving complex problems, have strong technical skills, and excel in client-facing roles, we'd love to hear from you.
    Position Overview:
    We are looking for a Data Scientist with a minimum of 2 years of hands-on experience in data analysis, machine learning, and statistical modeling. The ideal candidate will possess a strong skill set in Python, NumPy, Pandas, Scikit Learn, Tensorflow/Pytorch, Matplotlib, and SQL. In addition to technical proficiency, excellent communication skills are essential as this role involves direct interaction with clients. The successful candidate should have a proven track record of completing projects on time, be adept at working both independently and as part of a team, and be proactive in resolving conflicts.
    Key Responsibilities:
    • Collaborate with cross-functional teams to understand business objectives and translate them into data-driven solutions.
    • Collect, preprocess, and analyze large datasets to derive actionable insights and build predictive models.
    • Develop and deploy machine learning algorithms and models using Python, Tensorflow/Pytorch, and Scikit Learn.
    • Visualize data and present findings to clients and stakeholders using Matplotlib and other visualization tools.
    • Write efficient SQL queries to extract and manipulate data from relational databases.
    • Proactively identify and resolve conflicts or roadblocks in project execution.
    • Meet project deadlines and ensure high-quality deliverables.
    • Maintain up-to-date knowledge of industry trends and best practices in data science.
    Qualifications:
    • Bachelor's or Master's degree in a related field (e.g., Computer Science, Statistics, Data Science).
    • Minimum of 2 years of hands-on experience as a Data Scientist.
    • Strong proficiency in Python, NumPy, Pandas, Scikit Learn, Tensorflow/Pytorch, Matplotlib, and SQL.
    • Excellent verbal and written communication skills with the ability to explain complex technical concepts to non-technical stakeholders.
    • Proven ability to work in client-facing roles and maintain positive client relationships.
    • Track record of completing projects on time and within scope.
    • Ability to work independently and collaboratively in a team environment.
    • Strong problem-solving skills and attention to detail.
    • Proactive attitude and ability to handle conflict resolution effectively.
    Cultural Fit:
    As a member of the SciData Solutions team, candidates should align with company's cultural fit, which includes embracing diversity and inclusion, fostering a growth mindset, demonstrating a passion for technology, and exhibiting a commitment to corporate social responsibility.
    How to Apply:
    If you are a data enthusiast who thrives in a collaborative environment and meets the qualifications outlined above, we invite you to apply for this exciting opportunity. Please submit your resume, cover letter, and any relevant portfolio or project work to hr@scidatasolutions.com.
    SciData Solutions is an equal opportunity employer. We celebrate diversity and are committed to creating an inclusive environment for all employees."""

    interview_transcript = """
    Here's an interview transcript for Rahul, the Data Scientist candidate from PyroPulse Analytics, interviewing for the position at SciData Solutions. The transcript includes questions related to Rahul's skills and experience, focusing on his knowledge of PyTorch.

    ---

    **Interviewer (Kumar)**: Good afternoon, Rahul. Thank you for joining us today. To start, could you briefly describe your experience at PyroPulse Analytics and the types of projects you've worked on there?

    **Rahul**: Good afternoon, Kumar. Thank you for having me. At PyroPulse Analytics, I've been primarily involved in data analysis and machine learning projects. I've worked on projects related to customer segmentation, churn prediction, and recommendation systems using Python, Pandas, NumPy, and Scikit Learn.

    **Interviewer (Kumar)**: I noticed on your resume that you have experience with PyTorch. Could you tell me about a specific project where you applied PyTorch and neural networks?

    **Rahul**: Certainly, Kumar. I worked on a project to build a sentiment analysis model for customer reviews using PyTorch. We used a recurrent neural network (RNN) architecture for this task. The model performed well in sentiment classification, achieving an accuracy of around 85%.

    **Interviewer (Kumar)**: That's interesting. Can you explain how the RNN architecture works and why it was chosen for sentiment analysis?

    **Rahul**: Of course. RNNs are a type of neural network designed for sequential data, such as text. They work by maintaining a hidden state that captures information from previous time steps. This makes them well-suited for tasks where the order of data matters, like sentiment analysis, where the sentiment of a word often depends on the context of the previous words.

    **Interviewer (Kumar)**: In the job description, we mentioned that we're looking for candidates with strong knowledge in PyTorch and neural networks. Can you tell me about some challenges you faced while working on the sentiment analysis project?

    **Rahul**: Well, to be honest, while I was able to implement the RNN for the project, I faced challenges when it came to hyperparameter tuning and optimizing the model for better performance. It's an area I'm actively working on improving.

    **Interviewer (Kumar)**: Thank you for your honesty, Rahul. It's important to acknowledge areas for growth. Moving on, could you share an example of a project where you had to work in a team and how you handled conflicts or challenges that arose during the project?

    **Rahul**: Certainly. In a previous project at PyroPulse, our team had a disagreement about the choice of machine learning algorithm to use. Some team members favored decision trees, while others preferred neural networks. I facilitated a discussion, and we decided to try both approaches and compare their performance. It turned out to be a valuable learning experience, as we could objectively assess the strengths and weaknesses of each method.

    **Interviewer (Kumar)**: Finally, how do you think your experience aligns with SciData Solutions' culture of embracing diversity, fostering a growth mindset, and demonstrating a passion for technology?

    **Rahul**: I believe my diverse project experience and my commitment to continuous learning align well with SciData Solutions' cultural values. I'm always eager to explore new technologies and improve my skills, which I believe is essential in the ever-evolving field of data science.

    **Interviewer (Kumar)**: Thank you, Rahul, for sharing your experiences and insights. We appreciate your time today."""

    evaluation_template = """
    1. Summarize the performance of the candidate
    2. Evaluate the candidate along with a score on 5
    on 4 dimensions - technical fitness, cultural fitness, communication skills and track record and overall."""

    response = evaluate(
        role=role,
        experience_template=experience_template,
        evaluation_template=evaluation_template,
        job_description=job_description,
        interview_transcript=interview_transcript,
    )
    print(response)
