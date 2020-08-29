# LimeHomeTest api
The Api contains 3 methods: 
 * GetLocation(at), where at is coordinates in string format separeted by comma. 
    Method returns hotels by this coordinates
 * CreateBookings(booking), where booking is object that  contains fields: 
     from - date of the beggining,
     end - date of the end,
     hotelid - id of hotel
   Method returns message about succesfully or not succesfully saving
 * GetBookings(id), where id is hotel id. Method returns bookings for that hotel.

For realize api used .net core web api. 
InMemoryDatabase was chosen as the database because it is suitable for test purposes, but not for real project.
Log is also realize as test version.
App is covered by unit tests.
For testing app implemented swagger.
