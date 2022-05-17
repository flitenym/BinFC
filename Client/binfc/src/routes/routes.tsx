import React from "react";
import Dashboard from "../views/admin/default";
import Profile from "../views/admin/profile";

const routes = [
  {
    name: "Dashboard",
    layout: "/admin",
    path: "/default",
    component: Dashboard,
  },
  {
    name: "Profile",
    layout: "/admin",
    path: "/profile",
    component: Profile,
  },
];

export default routes;
