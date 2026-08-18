using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeAndChill.Models
{
    internal class StaffDocuments
    {
        //The name of the file
        public string FileName { get; set; } = string.Empty;

        //File extension(.pdf, .word, etc)
        public string FileExtension { get; set; } = string.Empty;

        //Who created the file
        public string Author { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        //File size in bytes
        public long FileSize { get; set; }

        //File size in bytes
        public DateTime UploadedOn { get; set; }

        public string ShareName { get; set; } = "staff-docs";
    }
}