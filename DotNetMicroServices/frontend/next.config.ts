import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  reactStrictMode: true,
  compiler: {
    emotion: true,
  },
  transpilePackages: ["@mui/material", "@mui/system"],
};

export default nextConfig;

