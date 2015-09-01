using System;
using System.IO;
using RestSharp;

namespace bgg_uploader
{
	class MainClass
	{
		//public static string baseEndPoint = "http://teststack.vagrant.scout.com:8081";
		public static string baseEndPoint = "http://localhost:8080";

		public static void Main (string[] args)
		{
			var rest = new RestSharp.RestClient (baseEndPoint);

			switch(args[0]) {
			case "games":
				SyncGameInfo (args [1], rest);
				break;
			case "ratings":
				SyncRatings (args [1], rest);
				break;
			case "updateRatings":
				UpdateRatings (args [1], rest);
				break;
			}
		}

		static void UpdateRatings(string directory, RestClient rest)
		{
			foreach(var file in Directory.GetFiles(directory)) {
				var fileInfo = new FileInfo (file);
				var bggId = fileInfo.Name.Replace (".json", "");
				var req = new RestRequest ("/topic/boardgames/bgg/" + bggId + "/rating/update");
				Console.WriteLine (req.Resource);
				var resp = rest.Put (req);
				Console.WriteLine (resp.StatusCode);
				Console.WriteLine (resp.Content);
				Console.WriteLine ();
			}
		}

		static void SyncRatings(string directory, RestClient rest)
		{
			foreach(var file in Directory.GetFiles(directory)) {
				var fileInfo = new FileInfo (file);
				var bggId = fileInfo.Name.Replace (".json", "");
				foreach (var line in File.ReadLines(file)) {
					var req = new RestRequest ("/topic/boardgames/bgg/" + bggId + "/rating");
					req.AddParameter ("text/json", line, ParameterType.RequestBody);
					Console.WriteLine (req.Resource);
					var resp = rest.Put (req);
					Console.WriteLine (resp.StatusCode);
					Console.WriteLine (resp.Content);
					Console.WriteLine ();

					if (resp.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
						break;
					}
				}
			}
		}

		static void SyncGameInfo (string directory, RestClient rest)
		{
			foreach (var file in Directory.GetFiles (directory)) {
				var info = new FileInfo (file);
				var request = new RestRequest ("/topic/boardgames/bgg/" + info.Name.Replace (".json", ""), Method.PUT);
				Console.WriteLine (request.Resource);
				request.AddParameter ("text/json", File.ReadAllText (file), ParameterType.RequestBody);
				Console.WriteLine ("Putting json from " + file);
				var resp = rest.Put (request);
				Console.WriteLine (resp.StatusCode);
				Console.WriteLine (resp.Content);
			}
		}
	}
}
