using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IHomeSliderRepository : IRepository<HomeSlider>
    {
        void Update(HomeSlider slider);
    }
}
