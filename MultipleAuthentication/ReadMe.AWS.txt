Instructions for configuring Amazon Web Service
------------------------------------------------

- Follow article https://snevsky.com/blog/dotnet-core-authentication-aws-cognito 
- For authorization we have used client_credential 
- The following is the C# code snippet from Postman

var client = new RestClient("https://principa.auth.ap-south-1.amazoncognito.com/oauth2/token");
client.Timeout = -1;

var request = new RestRequest(Method.POST);
request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

request.AddParameter("client_id", "2rtjvd629f86l0bosl5svu1ivj");
request.AddParameter("client_secret", "av6hgaqoetiqqmb56pk7rqutlmmdrii1qh4appp82pp7lg3m4v3");
request.AddParameter("grant_type", "client_credentials");
request.AddParameter("scope", "https://appsmart-api/app.access");

IRestResponse response = client.Execute(request);
Console.WriteLine(response.Content);