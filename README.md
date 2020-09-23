# Securities Exchange

This is my implementation of a securities exchange (stocks, bonds and ...). 
A market participant can place an order, if the order gets matched, it will 
then be executed. Each market move is traced and order status is recorded. 
While the market is open, any amount of clusters running this application 
will be able to share orders using the caching layer. All times recorded are
in UTC format.

- Database: Postgresql
- Caching layer: Redis