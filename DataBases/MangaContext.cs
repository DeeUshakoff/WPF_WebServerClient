using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using WPF_WebServerClient.ServerBackend;

using System.IO;
using Scriban;

namespace WPF_WebServerClient.DataBases
{
   
    [HttpController(nameof(MangaList))]
    internal class MangaList 
    {
        public byte[] GetBuffer()
        {
            throw new NotImplementedException();
        }

        [HttpGET("manga_id")]
        public Manga? GetManga(int manga_id)
        {
            var mangas = new List<Manga>();
            mangas.Add(new Manga() { Id = 1, Name = "Zalupa", Chapters = new List<Chapter> { new Chapter() { Number = 231 } } });

            return mangas.FirstOrDefault(t => t.Id == manga_id);
        }

        
    }


    public class Manga
    {
        


        //[HttpGET("chapter_id")]
        //public Chapter? GetChapter(int manga_id, int chapter_id)
        //{
        //    var mangas = new List<Manga>();
        //    mangas.Add(new Manga() { Id = 1, Name = "Zalupa", Chapters = new List<Chapter> { new Chapter() { Number = 231 } } });

        //    return mangas.FirstOrDefault(t => t.Id == manga_id && t.Chapters.Exists(x => x.Number == chapter_id)).Chapters.FirstOrDefault(x => x.Number == chapter_id);
        //}
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CoverImageLink { get; set; }
        /*public Rating*/
        public List<Chapter> Chapters { get; set; }
        
    }
    [HttpController(nameof(MangaTemplate))]
    public class MangaTemplate 
    {
        [HttpGET("")]
        public byte[] GetRenderBuffer()
        {
            return Encoding.UTF8.GetBytes(RenderTemplate(new Manga() { Id = 1, Name = "Ssanina", Description="Govno", CoverImageLink= "https://mangalib.me/uploads/cover/chainsaw-man/cover/mUIlgi4AJypL_250x350.jpg", Chapters = new List<Chapter> { new Chapter() { Number = 231 } } }));
        }
        public static string RenderTemplate(Manga manga) 
        {
            var temlpate_path = Directory.GetCurrentDirectory() + @"\Site\manga_profile.html";
            if (!File.Exists(temlpate_path)) return "govno";

            var page = File.ReadAllText(temlpate_path);
            var template = Template.Parse(page);
            var result = template.Render(new { manga_name = manga.Name, description = manga.Description, cover_image_link = manga.CoverImageLink });

            return result;
        }
        
    }
    public class Chapter
    {
        public int? Number { get; set; }
        public List<Page>? Pages { get; set; }
    }
    public class Page
    {
        public int Number { get; set; }
        public string PageURL { get; set; }

    }
    internal class Rating
    {
        private double rateValue;

        public int RateObjectID { get; set; }

        /*public double RateValue 
        { 
            get => rateValue;
            set
            {
                if(Math.Abs(rateValue + value) <0 || ) rateValue = 0;
            }
        }*/
    }
}
