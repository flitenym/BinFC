import React from "react";

import Content from "./components/Content";
import PropTypes from "prop-types";
import { Box, Drawer } from "@mui/material";

function Sidebar(props: any) {
  const { routes } = props;
  // SIDEBAR
  return (
    <Box> 
      <Content routes={routes} />
    </Box>
  );
}

// FUNCTIONS
export function SidebarResponsive(props: any) {
  const { routes } = props;

  return (
    <Box display={{ sm: "flex", xl: "none" }} alignItems='center'>
      <Drawer>
        <Content routes={routes} />
      </Drawer>
    </Box>
  );
}
// PROPS

Sidebar.propTypes = {
  logoText: PropTypes.string,
  routes: PropTypes.arrayOf(PropTypes.object),
  variant: PropTypes.string,
};

export default Sidebar;
