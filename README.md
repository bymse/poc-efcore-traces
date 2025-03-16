## TODO

application:
1. Setup simple web-api with swagger
2. Add datalayer with: multiple entities, 1to1 connection, 1toMany connections
3. Add repository that extract: full entity with find, entity with projection, aggregation 
4. There should be one query with performance problem 
5. Load generator to show problem query
6. Data generator for database size

infra:
1. Traces for ef core through diagnostic events
2. Check if opentelemetry libs are good enough for this task

devops:
1. Local otel collector
2. Local jaeger for traces 
3. Local prometheus for metrics
4. Local grafana for metrics
5. Through docker-compose