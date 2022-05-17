import React from "react";
import Brand from "./Brand";
import Links from "./Links";
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { Box, Grid } from "@mui/material";

function SidebarContent(props: any) {
  const { routes } = props;
  return (
    <Grid>
      <Brand />
      <Box>
        <Links routes={routes} />
      </Box>
    </Grid>
  );
}

export default SidebarContent;