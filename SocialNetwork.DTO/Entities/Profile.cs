namespace SocialNetwork.DTO.Entities
{
    public class Profile
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public string BackGround { get; set; }

        public string Gender { get; set; }

        public Profile()
        {

        }

        public Profile(string name, string image, string gender)
        {
            Gender = gender;
            Image = image;
            Name = name;
        }

        
    }
}