docker rmi ironclad -f
docker rmi ironclad_nginx -f

docker rm $(docker ps -aq) -f
docker network prune

docker-compose -f Docker\docker-compose.yml -f Docker\docker-compose.release.yml -p ironclad config -e version=0.0.2
docker-compose -f Docker\docker-compose.yml -f Docker\docker-compose.release.yml -p ironclad build -e version=0.0.2
docker-compose -f Docker\docker-compose.yml -f Docker\docker-compose.release.yml -p ironclad up -d --force-recreate --remove-orphans
