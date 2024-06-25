namespace Application.Exceptions;

public class ProductExistsException(long productId) 
    : ApplicationException($"Product with id {productId} already exists");