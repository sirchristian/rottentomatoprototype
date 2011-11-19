CREATE TABLE Movies
(
	MovieId INTEGER PRIMARY KEY,
	Name VARCHAR(128) NOT NULL, 
	TomatoCriticsScore INTEGER,
	ImageUrl VARCHAR(128),
	MPAARating VARCHAR(4),
	RottenTomatoesId INTEGER,
	ImdbId INTEGER
);

CREATE TABLE People
(
	PersonId INTEGER PRIMARY KEY,
	Name VARCHAR(128) NOT NULL
);

CREATE TABLE Roles
(
	MovieId INTEGER NOT NULL,
	PersonId INTEGER NOT NULL,
	RoleTitle VARCHAR(1024)
);

CREATE UNIQUE INDEX Roles_PrimaryKey ON Roles 
( 
	MovieId, 
	PersonId
);

CREATE TABLE Genres
(
	GenreId INTEGER PRIMARY KEY,
	Name VARCHAR(1024) NOT NULL
);

CREATE TABLE MovieGenres
(
	MovieId INTEGER NOT NULL,
	GenreId INTEGER NOT NULL
);

CREATE UNIQUE INDEX MovieGenres_PrimaryKey ON MovieGenres 
( 
	MovieId, 
	GenreId
);