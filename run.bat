docker stop eventstore-node
docker rm eventstore-node

docker kill location
docker kill proximity
docker kill member

docker rm location
docker rm proximity
docker rm member

docker network rm playground

docker network create playground

start /d "." docker run -p 1113:1113 -p 2113:2113 -it --rm --network=playground --name eventstore-node eventstore/eventstore 

cd dockerplayground.member.service
dotnet publish -c Release -o publish
cd..

docker build -t dockerplayground-member-service ./dockerplayground.member.service
start /d "." docker run -p 5000:80 --expose=5000 -it --rm --network=playground --name member dockerplayground.member.service

cd dockerplayground.eventstore.location
dotnet publish -c Release -o publish
cd..

docker build -t dockerplayground.eventstore.location ./dockerplayground.eventstore.location
start /d "." docker run -p 5001:80 --expose=5001 -it --rm --network=playground --name location dockerplayground.eventstore.location

cd dockerplayground.proximity.service
dotnet publish -c Release -o publish
cd..

docker build -t dockerplayground-proximity-service ./dockerplayground.proximity.service
start /d "." docker run -p 5002:80 --expose=5002 -it --rm --network=playground --name proximity dockerplayground.proximity.service
