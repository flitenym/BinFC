import React from "react";
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';

export function ItemContent(props: any) {
  return (
    <>
      <Box sx={{ flexDirection: 'column' }}>
        <Typography
          mb='5px'
          fontWeight='bold'
          color="text.primary"
          fontSize={{ base: "md", md: "md" }}>
          {props.title}
        </Typography>
        <Box alignItems='center'>
          <Typography
            fontSize={{ base: "sm", md: "sm" }}
            lineHeight='100%'
            color="text.secondary">
            {props.description}
          </Typography>
        </Box>
      </Box>
    </>
  );
}
