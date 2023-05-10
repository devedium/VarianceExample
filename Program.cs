namespace VarianceExample
{

    // Abstract Entity class
    public abstract class Entity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Active { get; set; }
    }

    // Customer class inheriting from Entity
    public class Customer : Entity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    // Order class inheriting from Entity
    public class Order : Entity
    {
        public string Code { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string PONumber { get; set; }
        public string TaxCode { get; set; }
    }

    public interface IReadOnlyRepository<out T> where T : Entity
    {
        IEnumerable<T> GetAll();
        T FindById(int Id);
    }

    public interface IRepository<T> : IReadOnlyRepository<T> where T : Entity
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
    }

    public class CustomerRepository : IRepository<Customer>
    {
        private readonly List<Customer> _customers = new List<Customer>();
        private readonly IEventPublisher<Customer> _publisher;
        public CustomerRepository(IEventPublisher<Customer> publisher)
        {
            _customers = new List<Customer>
            {
                new Customer
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Active = true,
                    Code = "CUST001",
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Phone = "555-1234",
                    Address = "123 Main St"
                },
                new Customer
                {
                    Id = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Active = true,
                    Code = "CUST002",
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    Phone = "555-5678",
                    Address = "456 Market St"
                }
            };
            _publisher = publisher;
        }
        public IEnumerable<Customer> GetAll()
        {
            return _customers;
        }

        public void Add(Customer entity)
        {
            _customers.Add(entity);
            _publisher?.OnCreate(entity);
        }

        public void Delete(Customer entity)
        {
            _customers.Remove(entity);
            _publisher?.OnDelete(entity);
        }

        public void Update(Customer entity)
        {
            var index = _customers.FindIndex(c => c.Id == entity.Id);
            if (index != -1)
            {
                _customers[index] = entity;
            }
            _publisher?.OnUpdate(entity);
        }

        public Customer FindById(int id)
        {
            return _customers.FirstOrDefault(c => c.Id == id);
        }
    }

    public class OrderRepository : IRepository<Order>
    {
        private readonly List<Order> _orders = new List<Order>();
        private readonly IEventPublisher<Order> _publisher;
        public OrderRepository(IEventPublisher<Order> publisher)
        {
            _orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Active = true,
                    Code = "ORD001",
                    OrderDate = DateTime.UtcNow,
                    DeliveryAddress = "123 Main St",
                    PONumber = "PO001",
                    TaxCode = "TX001"
                },
                new Order
                {
                    Id = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Active = true,
                    Code = "ORD002",
                    OrderDate = DateTime.UtcNow,
                    DeliveryAddress = "456 Market St",
                    PONumber = "PO002",
                    TaxCode = "TX002"
                }
            };
            _publisher = publisher;
        }
        public IEnumerable<Order> GetAll()
        {
            return _orders;
        }

        public void Add(Order entity)
        {
            _orders.Add(entity);
            _publisher?.OnCreate(entity);
        }

        public void Delete(Order entity)
        {
            _orders.Remove(entity);
            _publisher?.OnDelete(entity);
        }

        public void Update(Order entity)
        {
            var index = _orders.FindIndex(o => o.Id == entity.Id);
            if (index != -1)
            {
                _orders[index] = entity;
            }
            _publisher?.OnUpdate(entity);
        }

        public Order FindById(int id)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }
    }

    public class AuditLogger
    {
        public static void WriteAuditLog(IReadOnlyRepository<Entity> repository)
        {
            var entities = repository.GetAll();

            foreach (var entity in entities)
            {
                Console.WriteLine($"Entity Type: {entity.GetType().Name}");
                Console.WriteLine($"Id: {entity.Id}");
                Console.WriteLine($"Created at: {entity.CreatedAt}");
                Console.WriteLine($"Updated at: {entity.UpdatedAt}");
                Console.WriteLine($"Active: {entity.Active}");
                Console.WriteLine();
            }
        }
    }

    public interface IEventPublisher<in T> where T : Entity
    {
        void OnCreate(T entity);
        void OnDelete(T entity);
        void OnUpdate(T entity);
    }

    public class CustomerEventPublisher : IEventPublisher<Customer>
    {
        public void OnCreate(Customer entity)
        {
            Console.WriteLine($"Customer created: {entity.Name}");
        }

        public void OnDelete(Customer entity)
        {
            Console.WriteLine($"Customer deleted: {entity.Name}");
        }

        public void OnUpdate(Customer entity)
        {
            Console.WriteLine($"Customer updated: {entity.Name}");
        }
    }

    public class OrderEventPublisher : IEventPublisher<Order>
    {
        public void OnCreate(Order entity)
        {
            Console.WriteLine($"Order created: {entity.Code}");
        }

        public void OnDelete(Order entity)
        {
            Console.WriteLine($"Order deleted: {entity.Code}");
        }

        public void OnUpdate(Order entity)
        {
            Console.WriteLine($"Order updated: {entity.Code}");
        }
    }

    public class GenericEventPublisher : IEventPublisher<Entity>
    {
        public void OnCreate(Entity entity)
        {
            Console.WriteLine($"entity created: {entity.Id}");
        }

        public void OnDelete(Entity entity)
        {
            Console.WriteLine($"entity deleted: {entity.Id}");
        }

        public void OnUpdate(Entity entity)
        {
            Console.WriteLine($"entity updated: {entity.Id}");
        }
    }


    internal class Program
    {
        delegate void Test(object o);
        static void Main(string[] args)
        {
            Entity customerEntity = new Customer
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Active = true,
                Code = "CUST001",
                Name = "John Doe",
                Email = "john.doe@example.com",
                Phone = "555-1234",
                Address = "123 Main St"
            };

            Entity orderEntity = new Order
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Active = true,
                Code = "ORD001",
                OrderDate = DateTime.UtcNow,
                DeliveryAddress = "456 Market St",
                PONumber = "PO12345",
                TaxCode = "TX001"
            };

            LogEntity(customerEntity);
            LogEntity(orderEntity);

            Console.WriteLine($"customerEntity is Entity: {customerEntity is Entity}");
            Console.WriteLine($"orderEntity is Entity: {orderEntity is Entity}");

            IList<Customer> customerEntities = new List<Customer>
            {
                new Customer
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Active = true,
                    Code = "CUST001",
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Phone = "555-1234",
                    Address = "123 Main St"
                },
                new Customer
                {
                    Id = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Active = true,
                    Code = "CUST002",
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    Phone = "555-5678",
                    Address = "456 Market St"
                }
            };

            Console.WriteLine($"customerEntities is IList<Entity>: {customerEntities is IList<Entity>}");
            Console.WriteLine($"customerEntities is IList<Entity>: {customerEntities is IEnumerable<Entity>}");
            LogEntities(customerEntities);


            CustomerRepository customerRepository = new CustomerRepository(new CustomerEventPublisher());
            OrderRepository orderRepository = new OrderRepository(new OrderEventPublisher());

            IReadOnlyRepository<Entity> entityRepository = customerRepository;
            AuditLogger.WriteAuditLog(entityRepository);
            Console.WriteLine($"customerRepository is IRepository<Entity>: {customerRepository is IReadOnlyRepository<Entity>}");

            customerRepository.Add(new Customer
            {
                Id = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Active = true,
                Code = "CUST003",
                Name = "Alice Johnson",
                Email = "alice.johnson@example.com",
                Phone = "555-9876",
                Address = "789 Elm St"
            });

            Console.WriteLine($"GenericEventPublisher is IEventPublisher<Customer>: {new GenericEventPublisher() is IEventPublisher<Customer>}");

            customerRepository = new CustomerRepository(new GenericEventPublisher());

            customerRepository.Add(new Customer
            {
                Id = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Active = true,
                Code = "CUST007",
                Name = "Robert Brown",
                Email = "robert.brown@example.com",
                Phone = "555-9876",
                Address = "321 Oak St"
            });

            IEventPublisher<Entity> entityPublisher = new GenericEventPublisher();
            IEventPublisher<Customer> customerPublisher = entityPublisher;
            IEventPublisher<Order> orderPublisher = entityPublisher;
            entityPublisher.OnCreate(new Customer { Id = 1, Name = "John Doe" });

        }
        static void LogEntity(Entity entity)
        {
            Console.WriteLine($"Type: {entity.GetType().Name}");
            Console.WriteLine($"Id: {entity.Id}");
            Console.WriteLine($"Created at: {entity.CreatedAt}");
            Console.WriteLine($"Updated at: {entity.UpdatedAt}");
            Console.WriteLine($"Active: {entity.Active}");
            Console.WriteLine();
        }
        static void LogEntities(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                Console.WriteLine($"Id: {entity.Id}");
                Console.WriteLine($"Created at: {entity.CreatedAt}");
                Console.WriteLine($"Updated at: {entity.UpdatedAt}");
                Console.WriteLine($"Active: {entity.Active}");
                Console.WriteLine();
            }
        }

        public static void OnEntityCreated(Entity entity)
        {
            Console.WriteLine($"Entity {entity.Id} has been created.");
        }

        public static void OnEntityUpdated(Entity entity)
        {
            Console.WriteLine($"Entity {entity.Id} has been updated.");
        }

        public static void OnEntityDeleted(Entity entity)
        {
            Console.WriteLine($"Entity {entity.Id} has been deleted.");
        }
    }
}