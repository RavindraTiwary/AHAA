import os

from dotenv import load_dotenv
from langchain.chains import LLMChain
from langchain.chat_models import AzureChatOpenAI
from langchain.llms import AzureOpenAI, OpenAI
from langchain.prompts import PromptTemplate
from timestamp_removal import load_docx, merge_pages, remove_timestamps

load_dotenv()
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
OPENAI_DEPLOYMENT_NAME = os.getenv("OPENAI_DEPLOYMENT_NAME")


EVALUATION_PROMPT_TEMPLATE = """
You recently interviewed a candidate for a technical interview.
Given the job description and the interview transcript below, 
{evaluation_template}

Microsoft is an equal opportunity employer. All qualified applicants will receive consideration for employment without regard to age, ancestry, color, family or medical care leave, gender identity or expression, genetic information, marital status, medical condition, national origin, physical or mental disability, political affiliation, protected veteran status, race, religion, sex (including pregnancy), sexual orientation, or any other characteristic protected by applicable laws, regulations and ordinances.  We also consider qualified applicants regardless of criminal histories, consistent with legal requirements.

Give your output as a json with the keys as "summary" and "evaluation".
"summary" contains the performance summary of the candidate, and "evaluation" has the keys "technical_fitness", "cultural_fitness", "communication_skills", "track_record", "overall".
For each of the above evaluation keys, give your score under "score" (out of 5) and the reason for the score under "reason".
Along with these keys, it should also contain the key "inclusiveness" with the score and reason for the interviewer's inclusiveness.

Job Description: {job_description}

Interview Transcript: {interview_transcript}

Candidate Performance Summary and Evaluation:<json>"""


def evaluate(
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
            "evaluation_template": evaluation_template,
            "job_description": job_description,
            "interview_transcript": interview_transcript,
        }
    )
    return response


def evaluate_transcript(transcript_file_path, timestamp_pattern = r'\d+:\d+:\d+\.\d+ --> \d+:\d+:\d+\.\d+'):
    job_description = """
Job description

Requisition ID: 1556538

Job Title: Senior Consultant

Overview:

Microsoft Industry Solution - India Global Delivery Center (IGDC) delivers end-to-end solutions by enabling accelerated adoption and productive use of Microsoft technologies. An organization of well over 1000+ exceptional people, IGDC presents a great opportunity for highly skilled services professionals to make a foray into consulting, solution development and delivery roles. The ideal consultant is passionate for technology, has the breadth rather than specific product depth, and has the drive and courage to articulate and stand up for a 
great solution delivering true value for the client. 

  

As a Microsoft consultant, you will deliver quality engagements with your expertise, either as an advisor, reviewer, contributor, or resource in high profile projects to ensure customer value. The ideal candidate must have the ability to combine their technical skills, leadership skills, creativity, and customer focus to deliver great 
solutions to the customers and ensure they get the best out of our technologies and solutions. Consulting Delivery professionals bring subject matter and solution expertise to architectural teams, customers, and partners. They apply deep technical and business knowledge to accelerate the adoption of Microsoft devices and services by ensuring strategic, architectural, and operational alignment to customer and partner objectives.  

Job qualifications:

10+ years of experience 
Customer facing Project delivery leadership experience involving solution design, project envisioning, planning, development and deployment of complex solutions with minimum of 5 plus years. 
Bachelor’s Degree in Computer Science Engineering or equivalent work experience. Higher relevant education preferred. 
Technical certifications are a plus (MCSD/MCAD/MCSE/AZ-204) 

 

Microsoft is an equal opportunity employer. All qualified applicants will receive consideration for employment without regard to age, ancestry, color, family or medical care leave, gender identity or expression, genetic information, marital status, medical condition, national origin, physical or mental disability, political affiliation, protected veteran status, race, religion, sex (including pregnancy), sexual orientation, or any other characteristic protected by applicable laws, regulations and ordinances.  We also consider qualified applicants regardless of criminal histories, consistent with legal requirements. If you need assistance and/or a reasonable accommodation due to a disability during the application or the recruiting process, please send a request via the Accommodation request form.

 

Benefits/perks listed below may vary depending on the nature of your employment with Microsoft and the country where you work.

Job responsibilities:

 

Works as a Individual contributor or Leads the engineering team and provides accurate time estimates, sets work priorities, and makes project changes and trade-offs necessary for a successful release.  
Applies technical experience and industry-specific knowledge to develop solutions, based on an analysis of how the proposed approach affects the business objectives of customers and partners. 
Works to accelerate the value proposition of customer or partner engagements by helping to design, develop, and deploy solutions, based on Microsoft technologies and methodologies. 
Applies information-compliance and assurance policies to ensure stakeholder confidence. 
Responsible for the overall efficacy and quality of a project team’s technical delivery within his or her engagements. 
Defines dependencies and risks that go beyond the immediate scope and timeframe for a complex project. Develops contingency plans, risk-mitigation implementation criteria, and alternative strategies to manage short- and long-term risks and manages technical escalations. 
Identifies a best practice approach for a project, across a wide scope of technical issues, and develops or reuses intellectual capital with customers, world-wide, and for programs and initiatives across Microsoft. 
Drives opportunities to expand or accelerate the adoption and consumption of the cloud and Microsoft technologies.  Collaborates, as appropriate, with peers and other teams (e.g., Sales, account-aligned team) to scale the business with existing high-stake or strategic customers, by articulating/developing value propositions of strategic Microsoft products and services.  
Drives innovation and digital transformation. Ensures the use of the existing intellectual property (IP) and improves predictability. 
Defines the technology strategy for large and complex engagements, so that customers or partners realize the full value of investment. Responsible for implementing the technology strategy.  
Drives new ways of thinking, across the division and subsidiary, to improve quality, engineering productivity, and responsiveness to feedback and changing priorities. 

 

AREAS OF EXPERTISE:

 

.NET Tools and Technologies – experience of leveraging the same and designing, implementing, and deploying Business Mission Critical applications. 
Expertise in leading and delivering digital transformation projects. 
Exceptional coding abilities and experience with architectural patterns for large, high-scale applications on cloud platforms with focus on performance and resiliency. 
Extensive experience in implementing, operating, customizing, tuning and troubleshooting large-scale Cloud solutions.   
Full-stack developer with experience at many layers of the tech stack (native, web, service, UX, model, data) and good proficiency in one or more of C#, JavaScript, Typescript, AngularJS/ReactJS, HTML5, CSS/SCSS. 
Proven experience in creating reusable framework for application development addressing cross-cutting concerns. 
Excellence in software engineering practices, coding and solid foundation in data structures, algorithms with strong testing, debugging and analytical skills.  
Hands on experience with multi-threaded/parallel programming, Application design patterns, and anti-patterns, such as MVC, CQRS and/or SAGA   
Hands on experience with Application monitoring and end to end telemetry in the cloud. 
Proficient with one or more Cloud Databases such as Azure SQL, Azure Database for PostgreSQL, MySQL, and MariaDB   
Hands on experience in implementing micro-services architecture using technologies such as Kubernetes, Service Fabric, Cloud Foundry, Azure Functions  
Defining CI/CD pipelines to automate test and release across different application environments using concepts such as Blue/Green and Canary deployments and related technologies.  
Expertise in security controls such as encryption, AuthN/AuthZ  
Experience in Open-source technologies and frameworks are an added plus.   
Prior experience in Cloud migration, Java, AWS or Google cloud is a definite plus. 
Industry knowledge in one or more of the following industries: automotive, energy, travel and transportation, financial services, government, health, manufacturing, media & communications, or retail/supply chain.    """

    interview_transcript = remove_timestamps(transcript_file_path, transcript_file_path, timestamp_pattern, replace_multiple_newlines_whitespaces=True, load_n_merge=True)

    evaluation_template = """
    1. Summarize the performance of the candidate including candidate's name.
    2. Evaluate the candidate along with a score on 5
    on 4 dimensions - technical fitness based on the skillset mentioned in the Job Description, cultural fitness, communication skills and track record and overall.

    For the interviewer, give a score out of 5, to assess how well the interviewer performed on inclusiveness.
    
    Also provide a good description to justify your rating of the candidate. You should be aggressive with the rating criteria for the candidate."""

    response = evaluate(
        evaluation_template=evaluation_template,
        job_description=job_description,
        interview_transcript=interview_transcript,
    )
    print(response)

if __name__ == "__main__":
    transcript_file_path = r"C:\Users\nkothapalli\Downloads\Part 2-R1+R2 - Ratnesh Dixit - Interview Request - Senior Consultant (1556538) - September 29th, 2023_2023-09-29 (1).docx"
    evaluate_transcript(transcript_file_path)
