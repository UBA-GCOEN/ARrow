import userModel from "../models/userModel.js"
import bcrypt from 'bcrypt';
import jwt from 'jsonwebtoken'
import sendEmail from '../middlewares/sendEmail.js'
import generateToken from "../middlewares/generateToken.js";
import * as dotenv from "dotenv";
dotenv.config();



/**
 * Route: /sendEmail
 * Desc: send email with verification token
 */
export const sendResetEmail = async ( req, res) => {

      const email = req.body.email;

      /**
       * validate inputs for 
       * sql injection check
       */
      if(typeof email !== 'string'){
        console.log("invalid email")
        return 
      }

   try{

   

      /**
       * search the database 
       * for email
       */
      var emailFound = ''
      var SECRET = ''

      emailFound = await userModel.findOne({email});




      /**
       * assigning secret
       * based on role
       */
      if(emailFound){
        SECRET = process.env.USER_SECRET
      }
      else{
        res.send("no account found with the specified email")
      }

     const token = generateToken(emailFound, SECRET)

     const url = process.env.BASE_URL+`/verifyEmail?token=${token}&role=${emailFound.role}` 

      const options = {
        email: req.body.email,
        subject: "Forgot Password",
        body: url
      }

     await sendEmail(options);
     res.send("reset link sent")
    }
    catch(err){
        console.log(err)
    }
} 




/**
 * Route: /verifyEmail
 * Desc: verify email with token
 */
export const verifyEmail = async ( req, res) => {

    const token = req.query.token

    try{

    var decodeduser = ''
       
        decodeduser = await jwt.verify(token, process.env.USER_SECRET)

        if(decodeduser){

          const SECRET = process.env.USER_SECRET

          const oldUser = await userModel.findOne({email:decodeduser.email})

          const token = generateToken(oldUser, SECRET);


          req.session.user = {
            token: token,
            user: oldUser
          }

          console.log(req.session)

          res.status(200).json({
            success: true,
            msg: "user is logged in now make a post request to /updatePassword",
            result: oldUser,
            token,
            // csrfToken: req.csrfToken,
          });
        }
        else{
          res.status(404).send("User not found")
        }

    }
    catch(err){
        console.log(err)
    }
        
}


/**
 * Route: /updatePassword
 * Desc: change the password in DB
 */
export const updatePassword = async (req, res) => {


    const password = req.body.password
    const confirmPassword = req.body.confirmPassword

      //password constrains
      const passwordRegex = 
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*[@$%#^&*])(?=.*[0-9]).{8,}$/;
    
      // check password format

      if (!passwordRegex.test(password)) {
        return res.status(404).json({
       message: "Password must be at least 8 characters long and include at least 1 uppercase letter, 1 lowercase letter, 1 symbol (@$%#^&*), and 1 number (0-9)",
       });
     }            

       
     // check password match
      if(password != confirmPassword){
        res.json({msg:"Password does not match"})
        return
        }  
    
    try{  

        if(req.email){
            // hash password with bcrypt
            const hashedPassword = await bcrypt.hash(password, 12)      
            var result = '' 


            result = await userModel.updateOne({
                  password: hashedPassword
              })
            

            if(result){
                res.send("password changed successfully")
            }
        }
        else{
          res.status(400).json({
            msg: "unauthorized request"
          })
        }


   }
   catch(err){
    console.log(err)
   }  
    
        
}