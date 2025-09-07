DROP DATABASE IF EXISTS forecast;
CREATE DATABASE IF NOT EXISTS forecast;
USE forecast;
SELECT 'CREATING DATABASE STRUCTURE' as 'INFO';

DROP TABLE IF EXISTS weather;
DROP TABLE IF EXISTS calendar_event;


/*!50503 set default_storage_engine = InnoDB */;
/*!50503 select CONCAT('storage engine: ', @@default_storage_engine) as INFO */;



CREATE TABLE weather (
    id              INT             NOT NULL AUTO_INCREMENT,
    time_str        DATE            NOT NULL,
    temperature     FLOAT     NOT NULL,
    rain        FLOAT     NOT NULL,
    wind_speed      FLOAT     NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE calendar_event (
    id INT AUTO_INCREMENT PRIMARY KEY,
    event_id VARCHAR(20),
    data JSON NOT NULL
);

INSERT INTO weather (time_str, temperature, humidity, wind_speed) VALUES
    ('1972-05-13', 12.23, 321.21, 212.22),
    ('1972-05-14',12.23,321.21,212.22),
    ('1972-05-15',12.23,321.21,212.22),
    ('1972-05-16',12.21,321.21,212.22),
    ('1972-05-13',12.23,321.21,212.22);

SELECT * FROM calendar_event WHERE data->>'$.eTag' = NULL;


INSERT INTO calendar_event (event_id, data) VALUES
    ('sampel', '{ "a": 5 }'),
    ('sample2', '{ "a": 6 }');

CREATE USER 'user'@'localhost' IDENTIFIED WITH caching_sha2_password BY 'simplepwd';

GRANT ALL ON forecast.* TO 'user'@'localhost';

flush /*!50503 binary */ logs;
