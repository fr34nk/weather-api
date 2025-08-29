DROP DATABASE IF EXISTS forecast;
CREATE DATABASE IF NOT EXISTS forecast;
USE forecast;

SELECT 'CREATING DATABASE STRUCTURE' as 'INFO';

DROP TABLE IF EXISTS weather;

CREATE TABLE weather (
    id              INT       NOT NULL AUTO_INCREMENT,
    time_str        DATE      NOT NULL,
    temperature     FLOAT     NOT NULL,
    humidity        FLOAT     NOT NULL,
    wind_speed      FLOAT     NOT NULL,
);

INSERT INTO weather (time_str, temperature, humidity, wind_speed) VALUES
    ('1972-05-13',  12.23, 321.21, 212.22),
    ('1972-05-14',  12.23,321.21,212.22),
    ('1972-05-15',  12.23,321.21,212.22),
    ('1972-05-16',  12.21,321.21,212.22),
    ('1972-05-13',  12.23,321.21,212.22);

-     WHERE emp_no IN (4, 5, 6, 7);

CREATE USER 'user'@'localhost' IDENTIFIED WITH caching_sha2_password BY 'simplepwd';
GRANT ALL ON forecast.* TO 'user'@'localhost';

flush /*!50503 binary */ logs;
