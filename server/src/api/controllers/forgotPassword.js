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
      const role = req.body.role;

      /**
       * validate inputs for 
       * sql injection check
       */
      if(typeof email !== 'string'){
        console.log("invalid email")
        return 
      }
      if(typeof role !== 'string'){
        console.log("invalid role")
        return
      }

   try{

   

      /**
       * search the database 
       * for email
       */
      var emailFound = ''
      var SECRET = ''
      
      if(role == 'student'){
        emailFound = await userStudentModel.findOne({email});

      }
      else if(role == 'admin'){
        emailFound = await userAdminModel.findOne({email});
      }
      else if(role == 'staff'){
        emailFound = await userStaffModel.findOne({email});
      }
      else if(role == 'faculty'){
        emailFound = await userFacultyModel.findOne({email});
      }
      else if(role == 'visitor'){
        emailFound = await userVisitorModel.findOne({email});
      }
      else{
        res.send("invalid role")
      }
      console.log(emailFound.role)


      /**
       * assigning secret
       * based on role
       */
      if(emailFound){
        if(emailFound.role == 'admin'){
            SECRET = process.env.ADMIN_SECRET
        }
        else if(emailFound.role == 'student'){
            SECRET = process.env.STUDENT_SECRET
        }
        else if(emailFound.role == 'faculty'){
            SECRET = process.env.FACULTY_SECRET
        }
        else if(emailFound.role == 'staff'){
            SECRET = process.env.STAFF_SECRET
        }
        else if(emailFound.role == 'visitor'){
            SECRET = process.env.VISITOR_SECRET
        }
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
    const role = req.query.role

    try{

    var decodeduser = ''

        if(role == 'admin'){
            decodeduser = await jwt.verify(token,process.env.ADMIN_SECRET)
        }
        else if(role == 'student'){
            decodeduser = await jwt.verify(token, process.env.STUDENT_SECRET)
        }
        else if(role == 'faculty'){
            decodeduser = await jwt.verify(token, process.env.FACULTY_SECRET)
        }
        else if(role == 'staff'){
            decodeduser = await jwt.verify(token, process.env.STAFF_SECRET)
        }
        else if(role == 'visitor'){
            decodeduser = await jwt.verify(token, process.env.VISITOR_SECRET)
        }

        if(decodeduser.role){
            res.json({role: decodeduser.role})
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


    const role = req.body.role
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

        // hash password with bcrypt
        const hashedPassword = await bcrypt.hash(password, 12)      
        var result = '' 

        if(role == 'admin'){
          result = await userAdminModel.updateOne({
                password: hashedPassword
            })
        }
        else if(role == 'student'){
          result = await userStudentModel.updateOne({
                password: hashedPassword
            })
        }
        else if(role == 'faculty'){
            result = await userFacultyModel.updateOne({
                password: hashedPassword
            })
        }
        else if(role == 'staff'){
            result = await userStaffModel.updateOne({
                password: hashedPassword
            })
        }
        else if(role == 'visitor'){
            result = await userVisitorModel.updateOne({
                password: hashedPassword
            })
        }

        if(result){
            res.send("password changed successfully")
        }

   }
   catch(err){
    console.log(err)
   }  
    
        
}