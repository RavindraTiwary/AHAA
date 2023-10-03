import re
from langchain.document_loaders import Docx2txtLoader
from langchain.docstore.document import Document

def load_docx(file_path):
    loader = Docx2txtLoader(file_path)
    data = loader.load()
    return data

def merge_pages(pages):
    merged_pages = Document(
        page_content="\n".join([page.page_content for page in pages]),
        metadata=pages[0].metadata,
    )
    return merged_pages

def remove_timestamps(text, file_path=None, timestamp_pattern = r'\d+:\d+:\d+\.\d+ --> \d+:\d+:\d+\.\d+', replace_multiple_newlines_whitespaces=True, load_n_merge=True):
    if load_n_merge:
        if file_path is None:
            raise ValueError("file_path cannot be None if load_n_merge is True")
        pages = load_docx(file_path)
        merged_pages = merge_pages(pages)
        text = merged_pages.page_content

    clean_text = re.sub(timestamp_pattern, '', text)
    if replace_multiple_newlines_whitespaces:
        clean_text = re.sub('\n{3,}', '\n\n', clean_text)
        clean_text = re.sub(' +', ' ', clean_text)

    return clean_text

if __name__=="__main__":
#     text = """0:0:17.440 --> 0:0:28.200
# Ratnesh Kumar Dixit
# Manufacture part. So I was working on an interface where users will upload, upload their programme and then that will be saved into database and later while manufacturing.
# 0:0:29.520 --> 0:0:34.920
# Ratnesh Kumar Dixit
# Manufacturing unit. Can you access that and upload that programme? Then in the same.
# 0:0:50.460 --> 0:0:51.860
# Navpreet Singh
# Sorry to cut you in between.

#     """
    # print(remove_timestamps(text))
    file_path = r"C:\Users\nkothapalli\Downloads\Part 2-R1+R2 - Ratnesh Dixit - Interview Request - Senior Consultant (1556538) - September 29th, 2023_2023-09-29 (1).docx"
    timestamp_pattern = r'\d+:\d+:\d+\.\d+ --> \d+:\d+:\d+\.\d+'
    text = load_docx(file_path)
    text = merge_pages(text)
    print(remove_timestamps(text.page_content, timestamp_pattern, replace_multiple_newlines_whitespaces=True))