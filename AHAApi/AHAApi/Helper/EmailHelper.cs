using AHAApi.DataModels;
using AHAApi.Model;
using Azure.Core;
using System.Net.Http.Headers;

namespace AHAApi.Helper
{
    public class EmailHelper
    {
        private string accessToken;
        public EmailHelper()
        {
            accessToken = "EwCgA8l6BAAUAOyDv0l6PcCVu89kmzvqZmkWABkAAaFx8b8CIkdSRRhBPm0Jw51c33oTT0glG2GB/IhH8onK1nKeJzSlg2ciyDtkuEWx2kRVZby5XNipjKlyo5jOAgoFXkjtKaD7tJhsn3Ad3f9+0IiUfnuXwLztkHq5bA/MRj4d8Mi/B9Q7NYuQOcvIRmMERU2LD3Lwt3ifkfh0ky1rny5LOpYl35SFoQKyiHRaIsK+sqMowGQwBSfV0zMc2uO3q2J+1sapX6Lnv6GUP4c18IAX4697WySerSnYUUmEKYPQJbXzZx6sPmwvDbEBGzLaTrTUaJiVyzDvtYiQBsjUMhb8XiiCxlbuc7updDPr6cUW+oRhnpa9czU1lJ2j/Y4DZgAACCid7h7uUpP4cAJYp4H4dJDtU8YmuacftOWcnuHjP7RRt+Nbe6iyNWFlV98QMML9Z5B3r0+U3zkxp4c2fZZVwtAsHSb8SgcodFc75/86wriF5mGdxT2BON/8ZO0hG7kGIHdKjeE69yE0IvD/sJSBN18DK1JxLhqDdvW6L9oHCO6g4NksSumoyAiGQxDJTSfajYSQ1qEpxMAnzMdKqLJDfSuO8cisLCYZxR6KcY3OGaEYNjlc53tmRzo/zp0OnuBoAPYuz9oR3N1JVE0rO4h19ZZ1yTEEztm4H8ezHbrkjDuS2Nj9NZXXQRQuMKd+Va2OviCTkVhbHsw1kQXbX1lOQBiMjMhBut7k8hH4/Yt0/iHF7J59zV+MMMoAnEE99QxQaA5yw0vYxZajUd8whqY3aK//1XTGZruCs4XB0pIs1on+OWX1ee4pmtogrAHY8pPfmSNTNM2uR4g8UVzZ0Wvr76iFsU4PbxR24cq3WFMNw4HaD6JxH/or+loZpbQEyVdc8VZoxVdSMY5iVQANdPeNFcEIgNdzl1SL2mLOfVEdbXSmEEuxUoF8FvskHpW+WVoh6qbk8nBvrpcesoZRfbFZLwen/CwkFzywXy/k+gL3sdPW/SXFzaixY6pbytJrI/V1vAXUTpXlzXX6dlLbPTiz5ZJyeO7XEyJcOz94v2vvuynBSXve58Z16St8+2TultKohBfmyuJ0d9QsCpWSDLxj5/WziF22Jriq08jkuQlwqgQbvX1dw6Hn9gsazT6LCMkIILlFwZ5MSXjpZG28jzT27RAEAqqmUYp+4HH7ZZsEBxI90p3kcCIkx/OP6iJpcBTxkLqi7mOSqw/pvIGlAg==";
        }

        public void EmailUser(InterviewProfiles profile)
        {
            if (profile == null)
            {
                return;
            }

            EmailDetails emailDetails = new EmailDetails
            {
                CandidateName = profile.CandidateName ?? string.Empty,
                InterviewerName = profile.InterviewerName ?? string.Empty,
                InterviewProfileId = profile.JobId ?? string.Empty,
                SchedulingUrl = "https://www.google.com",
                JobTitle = profile.JobTitle ?? string.Empty,
                JobId = profile.JobId ?? string.Empty,
                JobDescription = profile.JobDescription ?? string.Empty
            };

            if (string.IsNullOrEmpty(profile.InterviewSlot1) && string.IsNullOrEmpty(profile.InterviewSlot2) && string.IsNullOrEmpty(profile.InterviewSlot3) && string.IsNullOrEmpty(profile.CandidateSelectedSlot))
            {
                emailDetails.EmailId = profile.InterviewerEmailId ?? string.Empty;
                emailDetails.IsRecipientInterviewer = true;
            }
            else if (string.IsNullOrEmpty(profile.CandidateSelectedSlot) && (!string.IsNullOrEmpty(profile.InterviewSlot1) || !string.IsNullOrEmpty(profile.InterviewSlot2) || !string.IsNullOrEmpty(profile.InterviewSlot3)))
            {
                emailDetails.EmailId = profile.CandidateEmailId ?? string.Empty;
            }

            SendEmail(emailDetails).Wait();
        }

        public async Task SendEmail(EmailDetails emailDetails)
        {
            if (emailDetails == null)
            {
                return;
            }

            if (emailDetails.IsRecipientInterviewer)
            {
                emailDetails.EmailSubject = "Provide the preferred slot for candidate" + " | Job title: " + emailDetails.JobTitle;
                emailDetails.EmailBody = $@"<p>Hi {emailDetails.InterviewerName},</p><p>Please click below to provide your preferred slot for the interview:</p><p><a href=\""{emailDetails.SchedulingUrl}\"">Click here to select a preferred time slot</a></p>
                                        <p>Candidate Name: {emailDetails.CandidateName} </p>
                                        <p>Job Id: {emailDetails.JobId} </p>
                                        <p>Job Title: {emailDetails.JobTitle} </p>
                                        <p>Job Description: {emailDetails.JobDescription} </p>
                                        <p>Regards,<br>AHAA-AI Hire Assistance Team</p>";

            }
            else
            {
                emailDetails.EmailSubject = "Select the preferred slot for interview | Job title: " + emailDetails.JobTitle;
                emailDetails.EmailBody = $@"
                                        <p>Hi {emailDetails.CandidateName},</p>
                                        <p>Please click below to choose your preferred interview slot with the interviewer:</p>
                                        <p><a href=\""{emailDetails.SchedulingUrl}\"">Click here to select your preferred time slot</a></p>
                                        <p>Interviewer Name: {emailDetails.InterviewerName} </p>
                                        <p>Job Id: {emailDetails.JobId} </p>
                                        <p>Job Title: {emailDetails.JobTitle} </p>
                                        <p>Job Description: {emailDetails.JobDescription} </p>
                                        <p>Regards,<br>AHAA-AI Hire Assistance Team</p>";
            }

            string emailJson = FormEmailJson(emailDetails);
            await CallEmailApi(emailJson);
        }

        private string FormEmailJson(EmailDetails emailDetails)
        {
            // Compose the email JSON with the provided recipient email
            return $@"
{{
    ""message"": {{
        ""subject"": ""{emailDetails.EmailSubject}"",
        ""body"": {{
            ""contentType"": ""HTML"",
            ""content"": ""{emailDetails.EmailBody}""
        }},
        ""toRecipients"": [
            {{
                ""emailAddress"": {{
                    ""address"": ""{emailDetails.EmailId}""
                }}
            }}
        ]
    }}
}}";
        }

        private async Task CallEmailApi(string emailJson)
        {
            // Make the API request
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await httpClient.PostAsync("https://graph.microsoft.com/v1.0/me/sendMail", new StringContent(emailJson, System.Text.Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        // Email sent successfully
                    }
                    else
                    {
                        // Handle error
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
