db.createUser(
        {
            user: "bookingDataUser",
            pwd: "password",
            roles: [
                {
                    role: "readWrite"
                }
            ]
        }
);