FROM nginx:alpine3.19-slim

RUN rm /etc/nginx/conf.d/default.conf

COPY reverse-proxy/nginx.conf /etc/nginx/nginx.conf

ENTRYPOINT [ "nginx", "-g", "daemon off;" ]
