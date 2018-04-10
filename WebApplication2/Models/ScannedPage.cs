using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class ScannedPage
    {
        private int spanCount;           
        private int linkCount;
        private int divCount;
        private string urlToSave;

        public int SpanCount { get => spanCount; set => spanCount = value; }
        public int LinkCount { get => linkCount; set => linkCount = value; }
        public int DivCount { get => divCount; set => divCount = value; }
        public string UrlToSave { get => urlToSave; set => urlToSave = value; }
    }
}