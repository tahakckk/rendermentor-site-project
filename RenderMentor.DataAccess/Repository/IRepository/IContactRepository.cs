using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IContactRepository : IRepository<Contact>
    {
        void Update(Contact contact);
    }
}
