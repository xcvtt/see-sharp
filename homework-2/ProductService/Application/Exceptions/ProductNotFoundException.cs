namespace Application.Exceptions;

public class ProductNotFoundException(long productId) 
    : ApplicationException($"Product with id {productId} not found");