namespace HighlyDeveloped.Core.ViewModel
{

    // This will hold the twitter tweet data for the lastest tweets
    public class TwitterViewModel
    {
        public string TwitterHandle { get; set; }
        public bool Error { get; set; }
        public string Json { get; set; }
        public string Message { get; set; }
    }
}
