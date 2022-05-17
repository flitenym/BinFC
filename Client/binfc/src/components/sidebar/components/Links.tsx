/* eslint-disable */
import { Typography } from "@mui/material";
import { Box } from "@mui/material";
import React from "react";
import { NavLink, useLocation } from "react-router-dom";

export function SidebarLinks(props: any) {
  //   Chakra color mode
  let location = useLocation();

  const { routes } = props;

  // verifies if routeName is the one active (in browser input)
  const activeRoute = (routeName: string) => {
    return location.pathname.includes(routeName);
  };

  // this function creates the links from the secondary accordions (for example auth -> sign-in -> default)
  const createLinks = (routes: any) => {
    return routes.map((route: any, index: any) => {
      if (route.category) {
        return (
          <>
            <Typography>
              {route.name}
            </Typography>
            {createLinks(route.items)}
          </>
        );
      } else if (
        route.layout === "/admin" ||
        route.layout === "/auth"
      ) {
        return (
          <NavLink to={route.layout + route.path}>
            {route.icon ? (
              <Box> 
                <Typography>
                  {route.name}
                </Typography>
              </Box>
            ) : (
              <Box> 
                <Typography>
                  {route.name}
                </Typography> 
              </Box>
            )}
          </NavLink>
        );
      }
    });
  };
  //  BRAND
  return createLinks(routes);
}

export default SidebarLinks;
