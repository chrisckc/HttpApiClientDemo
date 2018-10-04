using System;
using System.ComponentModel.DataAnnotations;

namespace DemoClient.Models
{
    public class ExampleModel
	{
		[Required]
        public string Heading { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        
    }
}