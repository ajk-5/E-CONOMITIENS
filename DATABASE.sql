DROP DATABASE IF EXISTS Economitiens;

CREATE DATABASE Economitiens;

USE Economitiens;

CREATE TABLE USER (
    ID SERIAL PRIMARY KEY,
    EMAIL VARCHAR(255) UNIQUE,
    FIRST_NAME VARCHAR(55),
    LAST_NAME VARCHAR(55),
    USER_NAME VARCHAR(255),
    PASSWORD VARCHAR(255),
    ROLE ENUM('TECHNICIEN', 'ADMINISTRATOR', 'USER')
);

CREATE TABLE ROOM (
    ROOM_NAME VARCHAR(45) PRIMARY KEY,
    BATIMENT VARCHAR(45),
    SENSORS_NUMBER INT
);

CREATE TABLE ELECTRICITY (
    ID SERIAL PRIMARY KEY,
    DATE_TIME_COLLECTED TIMESTAMP,
    CONSUMPTION INT,
    ROOM_NAME VARCHAR(45),
    FOREIGN KEY (ROOM_NAME) REFERENCES ROOM(ROOM_NAME)
);

CREATE TABLE HEAT (
    STATEMENT_ID SERIAL PRIMARY KEY,
    DATE_TIME_COLLECTED TIMESTAMP,
    TEMPERATURE FLOAT,
    ROOM_NAME VARCHAR(45),
    FOREIGN KEY (ROOM_NAME) REFERENCES ROOM(ROOM_NAME)
);

CREATE TABLE LIGHT (
    STATEMENT_ID SERIAL PRIMARY KEY,
    LIGHT_STATE BOOLEAN,
    DATE_TIME_COLLECTED TIMESTAMP,
    ROOM_NAME VARCHAR(45),
    FOREIGN KEY (ROOM_NAME) REFERENCES ROOM(ROOM_NAME)
);


CREATE TABLE HUMIDITY(
    ID SERIAL PRIMARY KEY,
    HUMIDITY INT,
    DATE_TIME_COLLECTED TIMESTAMP,
    ROOM_NAME VARCHAR(45),
    FOREIGN KEY (ROOM_NAME) REFERENCES ROOM(ROOM_NAME)
);

INSERT INTO ROOM (ROOM_NAME, BATIMENT) VALUES 
('E01', 'BâtimentE'),
('E03', 'BâtimentE'),
('E05', 'BâtimentE'),
('E06', 'BâtimentE'),
('E07', 'BâtimentE'),
('E08', 'BâtimentE'),
('E09', 'BâtimentE');