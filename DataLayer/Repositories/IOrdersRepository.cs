namespace DataLayer.Repositories;

public interface IOrdersRepository
{
    Task<OrderModel[]> GetAll();
    Task<OrderModel?> Find(int id);
}