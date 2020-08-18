declare @count int = (select count(id) from elias.CustomItem)
declare @maxtimes int = @count + 100
declare @type int = 5

while @count < @maxtimes
begin
	if @type > 7
		set @type = 5

	insert into Elias.CustomItem (Name, TypeId) values (CONCAT('Item', @count), @type)
	
	set @count = @count + 1
	set @type = @type + 1
end