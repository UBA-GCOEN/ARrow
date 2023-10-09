import userModel from "../models/userModel.js"
import bcrypt from 'bcrypt'
import generateToken from "../middlewares/generateToken.js"
import sendWelcomeMail from "../services/sendEmail.js";
import * as dotenv from "dotenv";
dotenv.config();

/**
 * Route: /user
 * Desc: to show or access user
 */
export const user = async (req, res) => {
    res.status(200).json({message:"Show user signin/signup page"})

}



/**
 * Route: /user/signup
 * Desc: user sign up
 */
export const signup = async (req, res, next) => {
       const { email, password, confirmPassword} = req.body

              
       //check if any field is not empty
       if ( !email || !password || !confirmPassword) {
        return res.status(404).json({
          success: false,
          msg: "Please Fill all the Details.",
        });
      }

      
      //password and email constrains
      const passwordRegex =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*[@$%#^&*])(?=.*[0-9]).{8,}$/;

      const emailDomains = [
        "@gmail.com",
        "@yahoo.com",
        "@hotmail.com",
        "@aol.com",
        "@outlook.com",
        "@gcoen.ac.in",
       ];

       // check email format
       if (!emailDomains.some((v) => email.indexOf(v) >= 0)) {
         return res.status(404).json({
        success: false,
        msg: "Please enter a valid email address",
      })};


      // check password format
       if (!passwordRegex.test(password)) {
         return res.status(404).json({
        success: false,
        msg: "Password must be at least 8 characters long and include at least 1 uppercase letter, 1 lowercase letter, 1 symbol (@$%#^&*), and 1 number (0-9)",
        });
      }            
 
        
      // check password match
       if(password != confirmPassword){
         res.json({
             success: false,
             msg:"Password does not match"})
         }    

       
         
         /**
          * checking field types
          * to avoid sql attacks
          */
        if (typeof email !== "string" && email !== undefined) {
          res.status(400).json({ status: "error" });
          return;
        }
  
        if (typeof password !== "string" || typeof confirmPassword !== "string") {
          res.status(400).json({ status: "error" });
          return;
        }         
       

       const oldUser = await userModel.findOne({ email });
       try{
        if(!oldUser){
 

          // hash password with bcrypt
           const hashedPassword = await bcrypt.hash(password, 12)
           
           // create user in database
            const result = await userModel.create({
                email,
                password: hashedPassword,
             });
    
             if(result){

              const oldUser = await userModel.findOne({email})

              const SECRET = process.env.USER_SECRET
              
              const token = generateToken(oldUser, SECRET);

              req.session.user = {
                token: token,
                user: oldUser
              }

              //send welcome email
              sendWelcomeMail(email)
    
              res.status(200).json({
                success: true,
                token,
                msg: "User added and logged in successfully"
              });
             }
           }
           else{
            res.status(403).json({
              success: false,
              msg: "user already exist"
            })
             
           }
      }
      catch(err){
        console.log(err)
      }

}




/**
 * Route: /user/signin
 * Desc: user sign in
 */
export const signin = async (req, res) => {
      const {email, password} = req.body  
      
      const oldUser = await userModel.findOne({email})

      const SECRET = process.env.USER_SECRET
      
      if(oldUser){
        
        const isPasswordCorrect = await bcrypt.compare(password, oldUser.password)

        if(isPasswordCorrect){
          
          const token = generateToken(oldUser, SECRET);

          req.session.user = {
            token: token,
            user: oldUser
          }
          
          res.status(200).json({
            success: true,
            token,
            _id: oldUser._id,
            isOnboarded: oldUser.isOnboarded,
            msg: "User is logged in successfully"
          });


        }
        else{
            req.session.destroy(err => {
              if (err) {
                console.error("Error destroying session:", err);
                res.status(500).send("Internal Server Error");
              } 
            });
            res.json({ msg: "Incorrect password" })
        }
      }
      else{
        req.session.destroy(err => {
          if (err) {
            console.error("Error destroying session:", err);
            res.status(500).send("Internal Server Error");
          } 
        });
        res.json({ msg:"User does not exist" })
      }
}






