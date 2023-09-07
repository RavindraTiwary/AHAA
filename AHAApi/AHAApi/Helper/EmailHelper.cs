using AHAApi.DataModels;
using AHAApi.Model;
using Azure.Core;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace AHAApi.Helper
{
    public class EmailHelper
    {
        private string accessToken;
        private string accessTokenOnlineMeeting;
        public EmailHelper()
        {
            accessToken = "EwCoA8l6BAAUAOyDv0l6PcCVu89kmzvqZmkWABkAAWR6+bN+PXLedTTPzH4ecW5gOvzTLiKkUPUQ0fHRRuKHV5GcFUaK34UgZPkiqV7RKrLHW7Z+WWvo7h4Fpu00g+Lx2P6x5Ht7DbFNLnsM4Vsq4HUeVYC+F0lwFvOkDuuroGLHqcYGUgVIg1Bwh2wPDqQE77eYsJkB4LUW9heh6mHvWRRtQSfOpFGeEEZwivC5wN/iX+LosFuaMuueuH5i2l5rxIGLDXE6JVvTqG8xYjoaSxUbOmka3cf7pQbKUZU0YHPcdMGyojnFK3b3/raBO3SJSTPo4Jhko4IEYsCV5Ok055mtcXvCoCqYX3idkMThZ+UHSLbWk+d4zif6rSE+sQ0DZgAACNeKxsohSbhueAJjKpKtUEmjrQBfDe6PMHtTE6YSQmNe6qelMzjuV3gYD1DuoAd5540VAL5hZ3n9U3n9OIrLq9C2+OoLOS2YGIGJzQuNYp0CwCd7zEf9l9sFY4lwjnUtPFR58TgTDnXgFbVCkygUAMrHoaKAEkW1wuAlrsfGOV67n9Z8j/BtFXqMlj3edolK9CKHUUrp09k88s34r4QapLonnckuGF2Cgx4804qBDMysm+e/C94qgavpYWm2u+eF6grrE0ZxtjC0796pQNSfw9Sg0OgD2XLfjXyXgaBvwrccADLEvO5IV7jgyJm7bOT/viPdh57cJfqCahrEFBU1TYq1qqPnLRc+NwzKrn+AtiVS3btbEK71hDEjk12Gh1V1jIanYKGIMs9x2tR0e2auXa1tRN8OHMsz6UaqR9w684SdXAWxOxkfp7175CjLMJVzp+br15dR7GD3ycNi+cgCDcVYosE0/jJmCS9mKAE4Xyf5THkFPhQRMWDJ3NMJWkPqDPk3wVixmIJfMk/188OeCBYzQFOWmc8PtAppTklN3E3anikpRakFSbSppElZsBvyPj5lpd+fddiYOXH5953IBmFt4SY3CBgSG/ElwbSlEvONYKz/lLybQwMczAIzH3j4ArqdKgQF3PUHulHd2HwByyEblUafUXDxhUtKYriiosq6lXI7KJa59fNYGQEH8Gfh0277R+ZOBsiSHNMr7MmUABg33WDNY3z2jDzdyzDe67+c9x8BgoECznIDEnVnd1p+/yEi1OknlYfRdZRxbmUm8N8GJQic4RDaAP1hPjBgQFiMvYyK594VIQQ9Go8mWJU8gk7r7Zl3IiAu0Lx0xcjPA/NK2qkC";
            accessTokenOnlineMeeting = "eyJ0eXAiOiJKV1QiLCJub25jZSI6IldCcVB5ODRFV0ZnVXpOWndXclZSYXQxWS1TZEQwT1RlTWlJLUxMSmloYXMiLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9kOTE0ZmE1Yy0wNzYwLTQ2MGQtYTA1Ni1hYzhhNzdhODdmZGEvIiwiaWF0IjoxNjkzOTgwMDc0LCJuYmYiOjE2OTM5ODAwNzQsImV4cCI6MTY5NDA2Njc3NCwiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhVQUFBQURydDZBLzlOam9DQ0srTll6Zk41OFdpMGgvSEd2cHkvRGxRNGZMNEY0RnlIOEY5UzM0c0ErUzRwS01Qbk0xbkNjS3ViSldiY1JYNU9vZHJTYzY5bXorY0pZYjd1ZGtwcUhqTWMyU2o4TTJzPSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoiR3JhcGggRXhwbG9yZXIiLCJhcHBpZCI6ImRlOGJjOGI1LWQ5ZjktNDhiMS1hOGFkLWI3NDhkYTcyNTA2NCIsImFwcGlkYWNyIjoiMCIsImZhbWlseV9uYW1lIjoiQWRtaW5pc3RyYXRvciIsImdpdmVuX25hbWUiOiJTeXN0ZW0iLCJpZHR5cCI6InVzZXIiLCJpcGFkZHIiOiIyNDA0OmY4MDE6ODAyODoxOmRhY2Y6ZWQwZjo1YTliOjYzNmIiLCJuYW1lIjoiU3lzdGVtIEFkbWluaXN0cmF0b3IiLCJvaWQiOiJjMDFhY2VmOS1kYWU0LTQ1OWEtODRhOC1iYzc0M2Q3NTQwM2MiLCJwbGF0ZiI6IjMiLCJwdWlkIjoiMTAwMzIwMDJEOTE4QzNEQyIsInJoIjoiMC5BYmNBWFBvVTJXQUhEVWFnVnF5S2Q2aF8yZ01BQUFBQUFBQUF3QUFBQUFBQUFBQzNBSzQuIiwic2NwIjoiQ2FsZW5kYXJzLlJlYWRXcml0ZSBPbmxpbmVNZWV0aW5ncy5SZWFkV3JpdGUgb3BlbmlkIHByb2ZpbGUgVXNlci5SZWFkIGVtYWlsIiwic3ViIjoia3lVdjJKWFZOdExGQkcwUU9tVkszVGpZUlNicTNUSkRjUVZ3MTBwSmRvbyIsInRlbmFudF9yZWdpb25fc2NvcGUiOiJOQSIsInRpZCI6ImQ5MTRmYTVjLTA3NjAtNDYwZC1hMDU2LWFjOGE3N2E4N2ZkYSIsInVuaXF1ZV9uYW1lIjoiYWRtaW5ATW5nRW52TUNBUDEzNjc0Mi5vbm1pY3Jvc29mdC5jb20iLCJ1cG4iOiJhZG1pbkBNbmdFbnZNQ0FQMTM2NzQyLm9ubWljcm9zb2Z0LmNvbSIsInV0aSI6IjAyWktNNklpWUU2MzRad1JjenMzQUEiLCJ2ZXIiOiIxLjAiLCJ3aWRzIjpbIjYyZTkwMzk0LTY5ZjUtNDIzNy05MTkwLTAxMjE3NzE0NWUxMCIsImI3OWZiZjRkLTNlZjktNDY4OS04MTQzLTc2YjE5NGU4NTUwOSJdLCJ4bXNfY2MiOlsiQ1AxIl0sInhtc19zc20iOiIxIiwieG1zX3N0Ijp7InN1YiI6Ikl4MjNSN0JqaExiS290TURHUVhLMUstenU2bW9NSW5KZG1pajZCdzhQMXcifSwieG1zX3RjZHQiOjE2OTIzNjExMDl9.rvR3B4n6B6llos7pdzjMkG1-r0_ybD9r6x0kovNb8-83qS6skmckO2AYEiZ62YZCuKsKy6B98kk3EUmM8N3YIJ-v4CiUnjTJow8xlBmf8VPd4A-l0yy9QaHztWAW9XjBh3jow2xteSyVjMB7Vo1z7AjrW3GPrmF8PRCrYrJ5ckVvYhm2cVwlOOwHWHVLvc1DTl5c5qXDAxJaXm51EBGL70MLx7YJv12MCxyOzk0e0oblBNLCjJISiX3HaG-RRhyx40jANMXe_wLBbXC6MuZga2aAsQex2kWdlrRF2gmPvVhKXgaWMzk-F1C60DKmBYN_Gc0QDAvOM62mbLr29CVbrw";
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
            else if (!string.IsNullOrEmpty(profile.CandidateSelectedSlot) && (!string.IsNullOrEmpty(profile.InterviewSlot1) || !string.IsNullOrEmpty(profile.InterviewSlot2) || !string.IsNullOrEmpty(profile.InterviewSlot3)))
            {
                emailDetails.ToBeInterviewScheduled = true;
                emailDetails.EmailId = profile.CandidateEmailId ?? string.Empty;
                emailDetails.InterviewerEmailId = profile.InterviewerEmailId ?? string.Empty;
                emailDetails.CandidateSelectedSlot = profile.CandidateSelectedSlot ?? string.Empty;
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
            else if (emailDetails.ToBeInterviewScheduled)
            {
                emailDetails.EmailSubject = "Interview scheduled" + " | Job title: " + emailDetails.JobTitle;
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

            if (emailDetails.ToBeInterviewScheduled)
            {
                await ScheduleInterview(emailDetails);
            }
            else
            {
                string emailJson = FormEmailJson(emailDetails);
                await CallEmailApi(emailJson);
            }
        }

        private async Task ScheduleInterview(EmailDetails emailDetails)
        {
            string joinUrl = string.Empty;
            string jobTitle = emailDetails.JobTitle;

            // Create a MeetingPayload object
            MeetingPayload payload = new MeetingPayload
            {
                subject = "Interview scheduled | Job title: " + jobTitle,
                recordAutomatically = true,
                allowTranscription = true
            };

            // Serialize the object to JSON
            string jsonString = JsonConvert.SerializeObject(payload);

            // Create an instance of HttpClient
            using (var httpClient = new HttpClient())
            {
                // Set the base URL for Microsoft Graph API
                httpClient.BaseAddress = new Uri("https://graph.microsoft.com/v1.0/");

                // Set the authorization header with the access token
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessTokenOnlineMeeting);

                // Set the content type header
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                // Create a StringContent with the JSON payload
                var content = new StringContent(jsonString);

                // Set the content type for the request
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // Make an HTTP POST request to create the online meeting
                HttpResponseMessage response = await httpClient.PostAsync("me/onlineMeetings", content);

                // Check if the request was successful (status code 201)
                if (response.IsSuccessStatusCode)
                {
                    // Parse and handle the response if needed
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var data = (JObject)JsonConvert.DeserializeObject(responseBody);
                    joinUrl = data.SelectToken(
                       "joinUrl").Value<string>();
                    Console.WriteLine("Online meeting created successfully.");
                }
                else
                {
                    // Handle the error response if the request fails
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }

            if (!string.IsNullOrEmpty(joinUrl))
            {
                string endDateTime = string.Empty;
                // Define variables for datetime, timezone, subject, URL, and email addresses
                string startDateTime = emailDetails.CandidateSelectedSlot;
                // Parse the input string to a DateTime object (assuming it's in a specific format)
                if (DateTime.TryParse(startDateTime, out DateTime utcDateTime))
                {
                    // Define the target time zone offset for UTC+5.30 (India Standard Time)
                    TimeSpan targetTimeZoneOffset = TimeSpan.FromHours(5.5); // 5 hours and 30 minutes

                    // Apply the target time zone offset to the UTC datetime
                    DateTime targetDateTime = utcDateTime + targetTimeZoneOffset;

                    // Format the targetDateTime as a string in the desired format
                    startDateTime = targetDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
                    // Add one hour to the selectedDateTime
                    DateTime oneHourLater = targetDateTime.AddHours(1);
                    endDateTime = oneHourLater.ToString("yyyy-MM-ddTHH:mm:ss");

                }

                string timeZone = "Asia/Kolkata";
                string teamsMeetingUrl = joinUrl ?? string.Empty;
                string emailAddress1 = emailDetails.InterviewerEmailId;
                string name1 = emailDetails.InterviewerName;
                string emailAddress2 = emailDetails.EmailId;
                string name2 = emailDetails.CandidateName;
                string emailAddress3 = "shahanshahnayyar@outlook.com";
                string name3 = "AHAA-AI Hire Assistant";

                // Define the JSON payload for creating the event with variables
                // Define the JSON payload for creating the event with variables
                string eventPayload = $@"
{{
    ""subject"": ""{emailDetails.EmailSubject}"",
    ""start"": {{
        ""dateTime"": ""{startDateTime}"",
        ""timeZone"": ""{timeZone}""
    }},
    ""end"": {{
        ""dateTime"": ""{endDateTime}"",
        ""timeZone"": ""{timeZone}""
    }},
    ""location"": {{
        ""displayName"": ""AHAA- Interview""
    }},
    ""body"": {{
        ""contentType"": ""HTML"",
        ""content"": ""<p>Hi,</p><p>Please click below link to join the interview</p><p><a href='{teamsMeetingUrl}'>Click here to join the interview</a></p>
                                        <p>Candidate Name: {emailDetails.CandidateName} </p>
                                        <p>Job Id: {emailDetails.JobId} </p>
                                        <p>Job Title: {emailDetails.JobTitle} </p>
                                        <p>Job Description: {emailDetails.JobDescription} </p>
                                        <p>Regards,<br>AHAA-AI Hire Assistance Team</p>""
    }},
    ""attendees"": [
        {{
            ""emailAddress"": {{
                ""address"": ""{emailAddress1}"",
                ""name"": ""{name1}""
            }},
            ""type"": ""required""
        }},
        {{
            ""emailAddress"": {{
                ""address"": ""{emailAddress2}"",
                ""name"": ""{name2}""
            }},
            ""type"": ""required""
        }},
        {{
            ""emailAddress"": {{
                ""address"": ""{emailAddress3}"",
                ""name"": ""{name3}""
            }},
            ""type"": ""required""
        }}
    ]
}}";

                // Create an instance of HttpClient
                using (var httpClient = new HttpClient())
                {
                    // Set the base URL for Microsoft Graph API
                    httpClient.BaseAddress = new Uri("https://graph.microsoft.com/v1.0/");

                    // Set the authorization header with the access token
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    // Set the content type header
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Create a StringContent with the JSON payload
                    var content = new StringContent(eventPayload);

                    // Set the content type for the request
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    // Make an HTTP POST request to create the event
                    HttpResponseMessage response = await httpClient.PostAsync("me/events", content);

                    // Check if the request was successful (status code 201)
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse and handle the response if needed
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Event created successfully.");
                    }
                    else
                    {
                        // Handle the error response if the request fails
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
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
