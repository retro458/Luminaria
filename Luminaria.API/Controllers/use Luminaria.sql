use Luminaria

alter table Auth.Usuarios
alter column ContraseñaHash nvarchar(500) not null

alter table Personajes
alter column FechaFallecimiento date null -- se cambia el tipo de dato


EXEC sp_ObtenerCatálogoPublico @Categoria = 'Programacion', @Busqueda = 'Ada';


select * from Categorias

CREATE FULLTEXT CATALOG ft_LuminariaCat AS DEFAULT;
GO

CREATE FULLTEXT INDEX ON Personajes(Nombre)
KEY INDEX PK__Personaj__FF7028BD8DF74C0A
WITH STOPLIST = SYSTEM; -- Ignora artículos comunes automáticamente (el, la, los)
GO

SELECT name 
FROM sys.key_constraints 
WHERE type = 'PK' AND parent_object_id = OBJECT_ID('dbo.Personajes');
EXEC sp_ObtenerCatálogoPublico @Categoria = NULL, @Busqueda = 'Ad';
