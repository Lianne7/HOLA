namespace HOLA.Models
{
    public class ForestViewModel
    {
        public int TreesPlanted { get; set; }

        public int EcoPoints { get; set; }

        public double CarbonOffset =>
            TreesPlanted * 0.25;
    }
}