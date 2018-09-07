select o.*,od.ProductID,p.ProductName, od.UnitPrice,od.Quantity,od.Discount
from dbo.Orders o
join dbo.[Order Details] od on o.OrderID = od.OrderID
join dbo.Products p on p.ProductID = od.ProductID
where o.OrderID = 10261
