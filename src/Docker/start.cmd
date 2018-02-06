docker-compose -f docker-compose.yml -f docker-compose.release.yml -p ironclad config
docker-compose -f docker-compose.yml -f docker-compose.release.yml -p ironclad build
docker-compose -f docker-compose.yml -f docker-compose.release.yml -p ironclad up -d --force-recreate --remove-orphans
docker rmi $(docker images -f "dangling=true" -q)