import React from 'react';
import { TextField, Button, Grid, Box, Stack, Paper, Typography } from '@mui/material';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { styled } from '@mui/material/styles';
import '@fontsource/roboto/700.css';

const Item = styled(Paper)(({ theme }) => ({
  backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
  ...theme.typography.body2,
  textAlign: 'left',
  color: theme.palette.text.secondary,
  boxShadow: 'none',
  paddingTop: '20px',
}));

const CustomTextField = styled(TextField)(({ theme }) => ({
  '& .MuiOutlinedInput-root': {
    '& fieldset': {
      borderColor: '#000',
      borderWidth: '2px',
    },
    '& input::-webkit-input-placeholder': {
      color: 'transparent',
    },
  },
}));

const CustomButton = styled(Button)(({ theme }) => ({
  borderRadius: theme.spacing(10),
  background: '#fff',
  border: '2px solid',
  borderColor: '#555',
  color: 'rgba(0, 0, 0, 0.6)',
  paddingTop: '15px',
  paddingBottom: '15px',
  textTransform: 'none',
  fontWeight: 'bold',
  fontSize: '1.1rem',
  boxShadow: '0',
  '&:hover': {
    boxShadow: '0',
    background: 'rgba(155, 155, 155, 0.1)',
  },
}));

const ComponentLoginForm: React.FC = () => {
  const theme = createTheme({
    typography: {
      h5: {
        fontWeight: 'bold',
        fontSize: '1.3rem',
      },
    },
  });

  return (
    <ThemeProvider theme={theme}>
      <Grid container justifyContent="center" alignItems="center">
        <Grid item xs={10} sm={8} md={6} xl={3}>
          <Box
            bgcolor="background.paper"
            borderRadius={10}
            boxShadow={0}
            p={8}
            paddingTop={2}
            paddingBottom={5}
            border={2}
            borderColor="grey.500"
            textAlign="center"
          >
            <form noValidate autoComplete="off">
              <Stack spacing={2}>
                <Item>
                  <Typography variant="h5" gutterBottom>
                    Username
                  </Typography>
                  <CustomTextField
                    fullWidth
                    id="username"
                    variant="outlined"
                    InputLabelProps={{ shrink: true }}
                  />
                </Item>
                <Item>
                  <Typography variant="h5" gutterBottom>
                    Password
                  </Typography>
                  <CustomTextField
                    fullWidth
                    id="password"
                    type="password"
                    variant="outlined"
                    InputLabelProps={{ shrink: true }}
                  />
                </Item>
              </Stack>
            </form>
          </Box>
          <Box mt={2}>
            <CustomButton fullWidth>Login</CustomButton>
          </Box>
        </Grid>
      </Grid>
    </ThemeProvider>
  );
};

export default ComponentLoginForm;
