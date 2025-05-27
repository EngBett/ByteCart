#!/bin/zsh

# ByteCart Docker Compose Helper Script

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

print_header() {
  echo "${BLUE}===========================================================${NC}"
  echo "${GREEN}$1${NC}"
  echo "${BLUE}===========================================================${NC}"
}

case "$1" in
  start|up)
    print_header "Starting ByteCart services..."
    docker-compose up -d
    echo "${GREEN}Services started! Access the application at http://localhost:8080${NC}"
    ;;
    
  stop|down)
    print_header "Stopping ByteCart services..."
    docker-compose down
    echo "${GREEN}Services stopped!${NC}"
    ;;
    
  restart)
    print_header "Restarting ByteCart services..."
    docker-compose down
    docker-compose up -d
    echo "${GREEN}Services restarted! Access the application at http://localhost:8080${NC}"
    ;;
    
  logs)
    print_header "Showing logs for ByteCart services..."
    if [ -z "$2" ]; then
      docker-compose logs -f
    else
      docker-compose logs -f "$2"
    fi
    ;;
    
  rebuild)
    print_header "Rebuilding and restarting ByteCart services..."
    docker-compose down
    docker-compose build --no-cache
    docker-compose up -d
    echo "${GREEN}Services rebuilt and restarted! Access the application at http://localhost:8080${NC}"
    ;;
    
  db)
    print_header "Accessing PostgreSQL database..."
    docker-compose exec bytecart-db psql -U super_admin -d ByteCart
    ;;
    
  clean)
    print_header "Cleaning up ByteCart Docker resources..."
    docker-compose down -v
    echo "${GREEN}Services and volumes removed!${NC}"
    ;;
    
  *)
    echo "ByteCart Docker Helper"
    echo "Usage: $0 [command]"
    echo ""
    echo "Commands:"
    echo "  start, up       Start services"
    echo "  stop, down      Stop services"
    echo "  restart         Restart services"
    echo "  logs [service]  View logs (optional: specify service name)"
    echo "  rebuild         Rebuild and restart services"
    echo "  db              Access PostgreSQL database"
    echo "  clean           Remove services and volumes"
    ;;
esac
