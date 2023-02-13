Instructions for configuring Azure AD
---------------------------------------

- Follow article https://dotnetplaybook.com/secure-a-net-core-api-using-bearer-authentication/ as it is
- For authorization we have used client_credential 
- The following is the C# code snippet from Postman

var client = new RestClient("https://login.microsoftonline.com/b945d629-94d4-40a3-a7bc-aa82c00329f6/oauth2/v2.0/token");
client.Timeout = -1;

var request = new RestRequest(Method.POST);
request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

request.AddParameter("client_id", "262e4f0e-c34b-4696-a6d0-4ad65e4c8cc1");
request.AddParameter("client_secret", "Z618Q~8_KyDZwkuZdE3RL47f384SYcDolsq2Pcor");
request.AddParameter("grant_type", "client_credentials");
request.AddParameter("scope", "api://1a03fe8d-7c81-4254-9b0e-7c1ef565c02a/.default");

IRestResponse response = client.Execute(request);
Console.WriteLine(response.Content);