create table Clients
(
	id int identity(1,1) not null,
	clientName nvarchar(15) not null,
	clientSurname nvarchar(15) not null,
	clientPatronymic nvarchar(15) not null,
	phone nvarchar(15) null,
	eMail nvarchar(20) not null
)

insert into Clients ([clientSurname], [clientName],[clientPatronymic],[phone],[eMail])
values (N'Фамилия 1',N'Имя 1', N'Отчество 1', '123-45-67', 'test1@mail.ru'),
	   (N'Фамилия 2',N'Имя 2', N'Отчество 2', null, 'test2@mail.ru'),
	   (N'Фамилия 3',N'Имя 3', N'Отчество 3', '987-65-43', 'test3@mail.ru'),
	   (N'Фамилия 4',N'Имя 4', N'Отчество 4', null, 'test4@mail.ru'),
	   (N'Фамилия 5',N'Имя 5', N'Отчество 5', '8(917)123-45-67', 'test5@mail.ru')
