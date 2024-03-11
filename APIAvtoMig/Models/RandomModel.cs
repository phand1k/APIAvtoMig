namespace APIAvtoMig.Models
{
    public class RandomModel : Random
    {
        public static int GetRandomNumber()
        {
            RandomModel random = new RandomModel();
            int code = random.Next(1000, 9999);
            return code;
        }
    }
}
