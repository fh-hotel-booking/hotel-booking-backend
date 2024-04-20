FROM mongodb/mongodb-community-server:6.0-ubi8
COPY ./mongodb-setup.js /docker-entrypoint-initdb.d/