using System;
namespace AspDotNet.Models
{
	public class EmailModel
	{
        public string? From { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? Date { get; set; }
	}
}