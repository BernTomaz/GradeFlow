FROM node:20.19-alpine AS build
WORKDIR /app

COPY src/GradeFlow.Web/package*.json ./
RUN npm ci

COPY src/GradeFlow.Web/ ./
RUN npm run build

FROM nginx:1.27-alpine
COPY src/GradeFlow.Web/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/GradeFlow.Web/browser /usr/share/nginx/html
EXPOSE 80
