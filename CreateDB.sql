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
	MovieId,
	PersonId,
	RoleTitle
);

CREATE UNIQUE INDEX Roles_PrimaryKey ON Roles 
( 
	MovieId, 
	PersonId
);