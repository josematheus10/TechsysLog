// https://angular.io/guide/build#proxying-to-a-backend-server

const PROXY_CONFIG = {
  '/api': {
    target: 'https://localhost:7063',
    changeOrigin: true,
    secure: false,
    logLevel: 'debug',
    pathRewrite: {
      '^/api': '/api'
    },
    onProxyReq: (proxyReq, req, res) => {
      proxyReq.setHeader('Origin', 'https://localhost:7063');
    },
    onProxyRes: (proxyRes, req, res) => {
      proxyRes.headers['Access-Control-Allow-Origin'] = '*';
      proxyRes.headers['Access-Control-Allow-Methods'] = 'GET, POST, PUT, DELETE, OPTIONS';
      proxyRes.headers['Access-Control-Allow-Headers'] = 'Origin, X-Requested-With, Content-Type, Accept, Authorization';
    },
    onError: (err, req, res) => {
    },
    bypass: function (req, res, proxyOptions) {
      return null;
    }
  },
  '/hubs': {
    target: 'https://localhost:7063',
    changeOrigin: true,
    secure: false,
    logLevel: 'debug',
    ws: true,
    onProxyReq: (proxyReq, req, res) => {
      proxyReq.setHeader('Origin', 'https://localhost:7063');
    },
    onProxyRes: (proxyRes, req, res) => {
    },
    onError: (err, req, res) => {
    }
  }
};

module.exports = PROXY_CONFIG;
