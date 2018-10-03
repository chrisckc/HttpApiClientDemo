using System;
using System.ComponentModel.DataAnnotations;

namespace DemoClient.Models
{
    public class DemoModel
	{
		[Required]
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        
    }
}