namespace AngularLib.Repository
{
    interface IForumRepository<T> where T:class
    {
        T GetItem(T item);
        T GetAuthorByItem(int itemId);
        T GetMostViewedItems();
        T GetRecentItems();
    }
}
